using System;
using System.Collections.Generic;
using GeoLoader.Properties;

namespace GeoLoader.Entities
{
    public class GeoCache
    {
        public int Id;
        public double Latitude;
        public double Longitude;
        public string Name;
        public string CacheContents;
        private string shortDescription;
        public string ShortDescription
        {
            get
            {
                var result = shortDescription;
                if (!string.IsNullOrEmpty(CacheContents))
                {
                    if (!string.IsNullOrEmpty(result))
                    {
                        result += "<br><br>";
                    }
                    result += "Содержимое тайника:" + CacheContents;
                }
                return result;
            }
            set
            {
                shortDescription = value;
            }
        }
        private string longDescription;
        public string LongDescription
        {
            get
            {
                if (Settings.Default.TruncateLongCacheDescriptions && longDescription != null &&
                    longDescription.Length > 8192)
                {
                    return longDescription.Substring(0, 8189) + "...";
                }
                return longDescription;
            }
            set
            {
                longDescription = value;
            }
        }
        public string Hints;
        public string Type
        {
            get
            {
                if (string.IsNullOrEmpty(TypeCode))
                {
                    throw new Exception("Тип не задан для кэша " + Id);
                }
                var type = "Unknown Cache";
                switch (TypeCode)
                {
                    case "TR":
                        type = "Traditional Cache";
                        break;
                    case "MS":
                        type = "Multi-cache";
                        break;
                    case "VI":
                        type = "Virtual Cache";
                        break;
                }
                return type;
            }
        }
        public string TypeCode;
        public string PlacedBy;
        public int PlacedById;
        public DateTime PlacedDate;
        public int Difficulty;
        public int Terrain;
        public string Country;
        public string State;
        public string Url;
        public bool Available = true;
        public bool Archived;
        public List<LogEntry> Log = new List<LogEntry>();
        public string CacheImage;
        public List<string> TerritoryImages;
        public string FullId
        {
            get { return TypeCode + Id; }
        }
    }
}
