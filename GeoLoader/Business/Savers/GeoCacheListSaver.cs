using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using GeoLoader.Entities;
using GeoLoader.Properties;

namespace GeoLoader.Business.Savers
{
    public class GeoCacheListSaver
    {
        private List<GeoCache> list;
        private bool poiStyle;
        public GeoCacheListSaver(List<GeoCache> list, bool poiStyle)
        {
            this.list = list;
            this.poiStyle = poiStyle;
        }

        public void Save(Stream stream)
        {
            var writer = XmlWriter.Create(stream);
            writer.WriteStartDocument();
            writer.WriteStartElement("gpx", "http://www.topografix.com/GPX/1/0");
            writer.WriteAttributeString("version", "1.0");
            writer.WriteAttributeString("creator", "GeoLoader http://rysenko.com/");
            writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
            writer.WriteAttributeString("xsi", "schemaLocation", null,
                                        "http://www.topografix.com/GPX/1/0 http://www.topografix.com/GPX/1/0/gpx.xsd http://www.groundspeak.com/cache/1/0 http://www.groundspeak.com/cache/1/0/cache.xsd");
            writer.WriteElementString("name", "Cache Listing Generated from Geocaching.Ru");
            writer.WriteElementString("desc", "This is a cache list generated from Geocaching.Ru");
            writer.WriteElementString("email", "org@geocaching.ru");
            writer.WriteElementString("url", "http://www.geocaching.su");
            writer.WriteElementString("urlname", "Geocaching - High Tech Treasure Hunting");
            writer.WriteElementString("keywords", "cache, geocache");
            /* This is no longer needed
            writer.WriteStartElement("bounds");
            writer.WriteAttributeString("minlat", "-90");
            writer.WriteAttributeString("minlon", "0");
            writer.WriteAttributeString("maxlat", "90");
            writer.WriteAttributeString("maxlon", "180");
            writer.WriteEndElement();*/
            foreach (var cache in list)
            {
                if (Settings.Default.SaveMinimalInfo && cache.TypeCode != "TR") continue;
                writer.WriteStartElement("wpt");
                writer.WriteAttributeString("lat", cache.Latitude.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("lon", cache.Longitude.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("time", cache.PlacedDate.ToString("s"));
                writer.WriteElementString("name", poiStyle ? cache.Name + " (" + cache.TypeCode + cache.Id + ")" : cache.TypeCode + cache.Id);
                writer.WriteElementString("desc", poiStyle ? GetPoiDescription(cache) : cache.Name);
                writer.WriteElementString("url", poiStyle && cache.CacheImage != null ? cache.Id + ".jpg" : cache.Url);
                writer.WriteElementString("urlname", cache.Name);
                writer.WriteElementString("sym", "Geocache");
                writer.WriteElementString("type", "Geocache|" + cache.Type);
                if (!poiStyle)
                {
                    writer.WriteStartElement("groundspeak", "cache", "http://www.groundspeak.com/cache/1/0");
                    writer.WriteAttributeString("id", cache.Id.ToString());
                    writer.WriteAttributeString("available", cache.Available ? "True" : "False");
                    writer.WriteAttributeString("archived", cache.Archived ? "True" : "False");
                    writer.WriteElementString("groundspeak", "name", null, cache.Name);
                    writer.WriteElementString("groundspeak", "placed_by", null, cache.PlacedBy);
                    writer.WriteStartElement("groundspeak", "owner", "http://www.groundspeak.com/cache/1/0");
                    writer.WriteAttributeString("id", cache.PlacedById.ToString());
                    writer.WriteString(cache.PlacedBy);
                    writer.WriteEndElement();
                    writer.WriteElementString("groundspeak", "type", null, cache.Type);
                    writer.WriteElementString("groundspeak", "difficulty", null, cache.Difficulty.ToString());
                    writer.WriteElementString("groundspeak", "terrain", null, cache.Terrain.ToString());
                    if (!string.IsNullOrEmpty(cache.Country))
                    {
                        writer.WriteElementString("groundspeak", "country", null, cache.Country);
                    }
                    if (!string.IsNullOrEmpty(cache.State))
                    {
                        writer.WriteElementString("groundspeak", "state", null, cache.State);
                    }
                    if (Settings.Default.SaveMinimalInfo)
                    {
                        writer.WriteElementString("groundspeak", "long_description", null, ReplaceEntities(cache.Hints));
                    }
                    else
                    {
                        writer.WriteStartElement("groundspeak", "short_description", "http://www.groundspeak.com/cache/1/0");
                        writer.WriteAttributeString("html", "True");
                        writer.WriteString(cache.ShortDescription);
                        writer.WriteEndElement();
                        writer.WriteStartElement("groundspeak", "long_description", "http://www.groundspeak.com/cache/1/0");
                        writer.WriteAttributeString("html", "True");
                        writer.WriteString(ReplaceEntities(cache.LongDescription));
                        writer.WriteEndElement();
                        writer.WriteStartElement("groundspeak", "encoded_hints", "http://www.groundspeak.com/cache/1/0");
                        writer.WriteString(HtmlToText(cache.Hints));
                        writer.WriteEndElement();
                        writer.WriteStartElement("groundspeak", "logs", "http://www.groundspeak.com/cache/1/0");
                        foreach (var logEntry in cache.Log)
                        {
                            writer.WriteStartElement("groundspeak", "log", "http://www.groundspeak.com/cache/1/0");
                            writer.WriteElementString("groundspeak", "date", null, logEntry.Date.ToString("s"));
                            writer.WriteElementString("groundspeak", "type", null, "Found it");
                            writer.WriteElementString("groundspeak", "finder", null, logEntry.Finder);
                            writer.WriteElementString("groundspeak", "text", null, ReplaceWrongXmlChars(logEntry.Text));
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
        }

        public string GetPoiDescription(GeoCache cache)
        {
            var result = string.Concat(
                "Доступность: ", cache.Difficulty, ", Местность: ", cache.Terrain, "<br>", ReplaceEntities(cache.Hints)
            );
            return result;
        }

        public string ReplaceWrongXmlChars(string text)
        {
            const string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(text, re, "");
        }

        public string HtmlToText(string htmlCode)
        {
            htmlCode = ReplaceEntities(htmlCode);
            htmlCode = htmlCode.Replace("\n", " ");
            htmlCode = htmlCode.Replace("\t", " ");
            htmlCode = Regex.Replace(htmlCode, "\\s+", " ");
            htmlCode.Replace("<br>", "\n<br>");
            htmlCode.Replace("<p>", "\n<p>");
            htmlCode.Replace("<br ", "\n<br ");
            htmlCode.Replace("<p ", "\n<p ");
            return Regex.Replace(htmlCode, "<[^>]*>", "").Trim();
        }

        public string ReplaceEntities(string htmlCode)
        {
            return System.Web.HttpUtility.HtmlDecode(htmlCode);
        }
    }
}
