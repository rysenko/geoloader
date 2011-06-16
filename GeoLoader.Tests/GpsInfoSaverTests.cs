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
        public void Geotag8352Image()
        {
            var cache = new GeoCacheLoader(8352).Load();
            var client = new Client();
            var imageData = client.DownloadData("http://www.geocaching.su/photos/caches/8352.jpg");
            File.WriteAllBytes("D:\\8352.jpg", imageData);
            GpsInfoSaver.WriteLongLat(File.ReadAllBytes("D:\\8352.jpg"), cache.Latitude, cache.Longitude);
        }

        [Test]
        public void Geotag8020Image()
        {
            var cache = new GeoCacheLoader(6641).Load();
            GpsInfoSaver.WriteLongLat(File.ReadAllBytes("D:\\6641.jpg"), cache.Latitude, cache.Longitude);
        }
    }
}
