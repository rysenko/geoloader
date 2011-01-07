using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using GeoLoader.Entities;

namespace GeoLoader.Business.Savers
{
    public class GeoCacheListSaver
    {
        private List<GeoCache> list;
        public GeoCacheListSaver(List<GeoCache> list)
        {
            this.list = list;
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
            writer.WriteStartElement("bounds");
            writer.WriteAttributeString("minlat", "-90");
            writer.WriteAttributeString("minlon", "0");
            writer.WriteAttributeString("maxlat", "90");
            writer.WriteAttributeString("maxlon", "180");
            writer.WriteEndElement();
            writer.WriteElementString("keywords", "cache, geocache");
            foreach (var cache in list)
            {
                writer.WriteStartElement("wpt");
                writer.WriteAttributeString("lat", cache.Latitude.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("lon", cache.Longitude.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("type", "Geocache|" + cache.Type);
                writer.WriteElementString("sym", "Geocache");
                writer.WriteElementString("name", cache.TypeCode + cache.Id);
                writer.WriteElementString("desc", cache.Name);
                writer.WriteElementString("urlname", cache.Name);
                writer.WriteElementString("url", cache.Url);
                /*writer.WriteStartElement("link");
                writer.WriteAttributeString("href", cache.Id + ".jpg");
                writer.WriteEndElement();*/
                writer.WriteElementString("time", cache.PlacedDate.ToString("s"));
                writer.WriteStartElement("groundspeak", "cache", "http://www.groundspeak.com/cache/1/0");
                writer.WriteAttributeString("id", cache.Id.ToString());
                writer.WriteAttributeString("available", cache.Available ? "True" : "False");
                writer.WriteAttributeString("archived", cache.Archived ? "True" : "False");
                writer.WriteElementString("groundspeak", "type", null, cache.Type);
                writer.WriteElementString("groundspeak", "name", null, cache.Name);
                writer.WriteElementString("groundspeak", "placed_by", null, cache.PlacedBy);
                writer.WriteStartElement("groundspeak", "owner", "http://www.groundspeak.com/cache/1/0");
                writer.WriteAttributeString("id", cache.PlacedById.ToString());
                writer.WriteString(cache.PlacedBy);
                writer.WriteEndElement();
                writer.WriteElementString("groundspeak", "country", null, cache.Country);
                if (!string.IsNullOrEmpty(cache.State))
                {
                    writer.WriteElementString("groundspeak", "state", null, cache.State);
                }
                writer.WriteElementString("groundspeak", "difficulty", null, cache.Difficulty.ToString());
                writer.WriteElementString("groundspeak", "terrain", null, cache.Terrain.ToString());
                writer.WriteElementString("groundspeak", "short_description", null, cache.ShortDescription);
                writer.WriteElementString("groundspeak", "long_description", null, ReplaceEntities(cache.LongDescription));
                writer.WriteStartElement("groundspeak", "encoded_hints", "http://www.groundspeak.com/cache/1/0");
                writer.WriteAttributeString("html", "True");
                writer.WriteString(HtmlToText(cache.Hints));
                writer.WriteEndElement();
                writer.WriteStartElement("groundspeak", "logs", "http://www.groundspeak.com/cache/1/0");
                foreach (var logEntry in cache.Log)
                {
                    writer.WriteStartElement("groundspeak", "log", "http://www.groundspeak.com/cache/1/0");
                    writer.WriteElementString("groundspeak", "type", null, "Found it");
                    writer.WriteElementString("groundspeak", "date", null, logEntry.Date.ToString("s"));
                    writer.WriteElementString("groundspeak", "finder", null, logEntry.Finder);
                    writer.WriteElementString("groundspeak", "text", null, logEntry.Text);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
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
