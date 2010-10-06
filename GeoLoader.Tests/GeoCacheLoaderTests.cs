using GeoLoader.Business.Loaders;
using NUnit.Framework;

namespace GeoLoader.Tests
{
    [TestFixture]
    public class GeoCacheLoaderTests
    {
        [Test]
        public void Load7357()
        {
            var cache = new GeoCacheLoader(7357).Load();
            Assert.AreEqual("Россия", cache.Country);
        }

        [Test]
        public void Load347()
        {
            var cache = new GeoCacheLoader(347).Load();
            Assert.AreEqual("Россия", cache.Country);
        }
    }
}
