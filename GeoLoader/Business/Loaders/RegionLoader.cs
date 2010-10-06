using System.Collections.Generic;
using System.Text.RegularExpressions;
using GeoLoader.Entities;

namespace GeoLoader.Business.Loaders
{
    public class RegionLoader : BaseLoader
    {
        public List<Region> List(int countryId)
        {
            var result = new List<Region>();
            var regionsData = client.DownloadString("http://pda.geocaching.su/list.php?c=" + countryId);
            var regionRegex = new Regex(@"<a href=""list.php\?c=\d+&a=(\d+)"">([^<]+)</a>");
            var regions = regionRegex.Matches(regionsData);
            foreach (Match region in regions)
            {
                result.Add(new Region
                               {
                                   Id = int.Parse(region.Groups[1].Value),
                                   Name = region.Groups[2].Value
                               });
            }
            return result;
        }
    }
}