using System.IO;
using GeoLoader.Business;
using GeoLoader.Business.Loaders;
using GeoLoader.Business.Savers;
using NUnit.Framework;

namespace GeoLoader.Tests
{
    [TestFixture]
    public class GpsInfoSaverTests
    {
        [Test]
        public void Geotag5965Image()
        {
            var cache = new GeoCacheLoader(5965).Load();
            var client = new Client();
            var imageData = client.DownloadData("http://www.geocaching.su/photos/caches/5965.jpg");
            File.WriteAllBytes("D:\\5965.jpg", imageData);
            GpsInfoSaver.WriteLongLat("D:\\5965.jpg", cache.Latitude, cache.Longitude);
        }

        [Test]
        public void Geotag8020Image()
        {
            var cache = new GeoCacheLoader(6641).Load();
            GpsInfoSaver.WriteLongLat("D:\\6641.jpg", cache.Latitude, cache.Longitude);
        }
    }
}
