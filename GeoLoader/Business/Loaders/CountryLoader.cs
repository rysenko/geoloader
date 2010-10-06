using System.Collections.Generic;
using System.Text.RegularExpressions;
using GeoLoader.Entities;

namespace GeoLoader.Business.Loaders
{
    public class CountryLoader : BaseLoader
    {
        public List<Country> List()
        {
            var result = new List<Country>();
            var countriesData = client.DownloadString("http://pda.geocaching.su/list.php");
            var countryRegex = new Regex(@"<a href=""list.php\?c=(\d+)"">([^<]+)</a>");
            var countries = countryRegex.Matches(countriesData);
            foreach (Match country in countries)
            {
                result.Add(new Country
                               {
                                   Id = int.Parse(country.Groups[1].Value),
                                   Name = country.Groups[2].Value
                               });
            }
            return result;
        }
    }
}