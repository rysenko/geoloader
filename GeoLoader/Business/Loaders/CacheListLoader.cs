using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GeoLoader.Business.Loaders
{
    public class CacheListLoader : BaseLoader
    {
        public List<int> List(int countryId, int regionId)
        {
            var result = new List<int>();
            var skip = 0;
            bool cachesFound;
            do
            {
                var url = string.Format("http://pda.geocaching.su/list.php?c={0}&a={1}&skip={2}",
                                        countryId, regionId, skip);
                var cachesData = client.DownloadString(url);
                var cacheRegex = new Regex(@"<a href=""cache.php\?cid=(\d+)""><b>[^<]+</b></a>");
                var caches = cacheRegex.Matches(cachesData);
                cachesFound = false;
                foreach (Match cache in caches)
                {
                    result.Add(int.Parse(cache.Groups[1].Value));
                    cachesFound = true;
                }
                skip += 20;
            } while (cachesFound);
            return result;
        }
    }
}