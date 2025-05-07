using System.Collections.Generic;
using GeoLoader.Entities;

namespace GeoLoader.Business.Loaders
{
    public class CountryLoader : BaseLoader
    {
        public List<Country> List()
        {
            var result = new List<Country>
            {
                new Country {Id = 1, Name = "Азербайджан"},
                new Country {Id = 2, Name = "Армения"},
                new Country {Id = 3, Name = "Беларусь"},
                new Country {Id = 4, Name = "Грузия"},
                new Country {Id = 5, Name = "Казахстан"},
                new Country {Id = 6, Name = "Киргизия"},
                new Country {Id = 7, Name = "Латвия"},
                new Country {Id = 8, Name = "Литва"},
                new Country {Id = 9, Name = "Молдова"},
                new Country {Id = 10, Name = "Россия"},
                new Country {Id = 11, Name = "Таджикистан"},
                new Country {Id = 13, Name = "Узбекистан"},
                new Country {Id = 14, Name = "Украина"},
                new Country {Id = 15, Name = "Эстония"},
            };
            //result.Add(new Country {Id = 0, Name = "Дальнее зарубежье"});
            return result;
        }
    }
}