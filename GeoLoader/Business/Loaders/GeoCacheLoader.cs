using System;
using System.Collections.Generic;
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
            cache.Country = GetFieldValue("Страна", true);
            cache.State = GetFieldValue("Область", false);
            cache.Difficulty = int.Parse(GetFieldValue("Доступность", true));
            cache.Terrain = int.Parse(GetFieldValue("Местность", true));
            var coordinates = GetFieldValue(@"Координаты \(WGS 84\)", true).Replace("<font class=coords>", "").Replace("</font>", "");
            var coordRegex = new Regex(@"([NS]) (\d{1,2})&#176; (\d{1,2}.\d{3})' &nbsp;&nbsp;&nbsp;([EW]) (\d{1,3})&#176; (\d{1,2}.\d{3})'");
            var coordMathResult = coordRegex.Match(coordinates);
            if (!coordMathResult.Success)
            {
                throw new Exception("Ошибка парсинга координат для кэша " + cacheId);
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
            cache.CacheContents = GetBlockValue("Содержимое тайника", false);
            cache.Hints = GetBlockValue("Описание тайника", false);
            var nameRegex = new Regex(@"<p><b>([^<]+)</b> от <b><a href=""profile.php\?uid=(\d+)"">'(.+?)'</a></b>(?:<br>|\s+)<i>\([^<]+? (\w{2})" + cacheId + @"\)</i>");
            var nameMathResult = nameRegex.Match(cacheData);
            if (!nameMathResult.Success)
            {
                throw new Exception("Ошибка парсинга имени и автора для кэша " + cacheId);
            }
            cache.Name = nameMathResult.Groups[1].Value;
            cache.PlacedById = int.Parse(nameMathResult.Groups[2].Value);
            cache.PlacedBy = nameMathResult.Groups[3].Value;
            cache.TypeCode = nameMathResult.Groups[4].Value;
            var created = GetFieldValue("Создан", true);
            cache.PlacedDate = DateTime.ParseExact(created, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            
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

            // Load images
            cacheData = client.DownloadString("http://pda.geocaching.su/pict.php?cid=" + cacheId + "&mode=0");
            var cacheImages = GetBlockImages("Фотография тайника");
            if (cacheImages.Count > 0)
            {
                cache.CacheImage = cacheImages[0];
            }
            cache.TerritoryImages = GetBlockImages("Фотографии местности");
            return cache;
        }

        string GetFieldValue(string fieldName, bool required)
        {
            var fieldRegex = new Regex(fieldName + @": <b>(.+?)</b>");
            var matchResult = fieldRegex.Match(cacheData);
            if (!matchResult.Success && required)
            {
                throw new Exception("Поле " + fieldName + " не найдено для кэша " + cacheId);
            }
            return matchResult.Groups[1].Value;
        }

        string GetBlockValue(string blockName, bool required)
        {
            var blockRegex = new Regex("<b><u>" + blockName + @"</u></b>(<br>)?([\w\W]+?)<p><hr>");
            var matchResult = blockRegex.Match(cacheData);
            if (!matchResult.Success && required)
            {
                throw new Exception("Блок " + blockName + " не найден для кэша " + cacheId);
            }
            return matchResult.Groups[2].Value;
        }

        List<string> GetBlockImages(string blockName)
        {
            var blockRegex = new Regex("<b>" + blockName + @":</b><br>([\w\W]+?)<hr>");
            var matchResult = blockRegex.Match(cacheData);
            if (!matchResult.Success)
            {
                throw new Exception("Блок " + blockName + " не найден для кэша " + cacheId);
            }
            var imagesData = matchResult.Groups[1].Value;
            var imagesRegex = new Regex(@"href=""([^""]+)""");
            var matchesResult = imagesRegex.Matches(imagesData);
            var resultingList = new List<string>();
            foreach (Match match in matchesResult)
            {
                resultingList.Add(match.Groups[1].Value);
            }
            return resultingList;
        }
    }
}