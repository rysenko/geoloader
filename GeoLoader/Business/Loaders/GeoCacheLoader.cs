using System;
using System.Globalization;
using System.Text.RegularExpressions;
using GeoLoader.Entities;
using GeoLoader.Properties;

namespace GeoLoader.Business.Loaders
{
    public class GeoCacheLoader : BaseLoader
    {
        private string cacheData;
        private int cacheId;

        public GeoCacheLoader(int cacheId)
        {
            this.cacheId = cacheId;
        }

        public GeoCache Load()
        {
            var cache = new GeoCache {Id = cacheId, Url = "http://pda.geocaching.su/cache.php?cid=" + cacheId};
            cacheData = client.DownloadString(cache.Url);
            cache.Country = GetFieldValue("Страна");
            cache.State = GetFieldValue("Область");
            cache.Difficulty = int.Parse(GetFieldValue("Доступность"));
            cache.Terrain = int.Parse(GetFieldValue("Местность"));
            var coordinates = GetFieldValue(@"Координаты \(WGS 84\)").Replace("<font class=coords>", "").Replace("</font>", "");
            var coordRegex = new Regex(@"([NS]) (\d{1,2})&#176; (\d{1,2}.\d{3})' &nbsp;&nbsp;&nbsp;([EW]) (\d{1,2})&#176; (\d{1,2}.\d{3})'");
            var coordMathResult = coordRegex.Match(coordinates);
            if (!coordMathResult.Success)
            {
                throw new Exception("Error parsing coordinates for cache " + cacheId);
            }
            cache.Latitude = int.Parse(coordMathResult.Groups[2].Value) +
                           double.Parse(coordMathResult.Groups[3].Value, CultureInfo.InvariantCulture) / 60;
            if (coordMathResult.Groups[1].Value == "S")
            {
                cache.Latitude = -cache.Latitude;
            }
            cache.Longitude = int.Parse(coordMathResult.Groups[5].Value) +
                           double.Parse(coordMathResult.Groups[6].Value, CultureInfo.InvariantCulture) / 60;
            if (coordMathResult.Groups[4].Value == "W")
            {
                cache.Longitude = -cache.Longitude;
            }
            cache.ShortDescription = GetBlockValue("Атрибуты", false);
            cache.LongDescription = GetBlockValue("Описание окружающей местности", true);
            cache.Hints = GetBlockValue("Описание тайника", false);
            var nameRegex = new Regex(@"<p><b>([^<]+)</b> от <b><a href=""profile.php\?uid=(\d+)"">'(.+?)'</a></b><br><i>\(\w+? (\w{2})" + cacheId + @"\)</i>");
            var nameMathResult = nameRegex.Match(cacheData);
            if (!nameMathResult.Success)
            {
                throw new Exception("Error parsing name and author of cache " + cacheId);
            }
            cache.Name = nameMathResult.Groups[1].Value;
            cache.PlacedById = int.Parse(nameMathResult.Groups[2].Value);
            cache.PlacedBy = nameMathResult.Groups[3].Value;
            cache.TypeCode = nameMathResult.Groups[4].Value;
            var created = GetFieldValue("Создан");
            cache.PlacedDate = DateTime.Parse(created);
            
            // Load log
            var logData = client.DownloadString("http://pda.geocaching.su/note.php?cid=" + cacheId);
            var logRegex = new Regex(@"<b><u>([^<]+)</u></b><i> от ([0-9\.]+)</i><br> ([\w\W]+?)<br>(<p>)?<hr>");
            var logResult = logRegex.Matches(logData);
            var logsAdded = 0;
            foreach (Match logMatch in logResult)
            {
                if (logsAdded < Settings.Default.MaxLogEntriesToSave)
                {
                    cache.Log.Add(new LogEntry
                    {
                        Finder = logMatch.Groups[1].ToString(),
                        Text = logMatch.Groups[3].ToString(),
                        Date =
                            DateTime.ParseExact(logMatch.Groups[2].ToString(), "dd.MM.yyyy",
                                                CultureInfo.InvariantCulture)
                    });
                    logsAdded++;
                }
            }
            return cache;
        }

        string GetFieldValue(string fieldName)
        {
            var fieldRegex = new Regex(fieldName + @": <b>(.+?)</b>");
            var matchResult = fieldRegex.Match(cacheData);
            if (!matchResult.Success)
            {
                throw new Exception("Field " + fieldName + " not found for cache " + cacheId);
            }
            return matchResult.Groups[1].Value;
        }

        string GetBlockValue(string blockName, bool required)
        {
            var blockRegex = new Regex("<b><u>" + blockName + @"</u></b>(<br>)?([\w\W]+?)<p><hr>");
            var matchResult = blockRegex.Match(cacheData);
            if (!matchResult.Success && required)
            {
                throw new Exception("Block " + blockName + " not found for cache " + cacheId);
            }
            return matchResult.Groups[2].Value;
        }
    }
}