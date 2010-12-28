using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace GeoLoader.Business.Loaders
{
    public class WptListLoader : BaseLoader
    {
        public List<int> List(string filePath)
        {
            var result = new List<int>();
            var wptContent = File.ReadAllText(filePath);

            var cacheRegex = new Regex(@"[0-9]+,[A-Z]{2}([0-9]+),");
            var caches = cacheRegex.Matches(wptContent);
            foreach (Match cache in caches)
            {
                result.Add(int.Parse(cache.Groups[1].Value));
            }
            return result;
        }
    }
    
}