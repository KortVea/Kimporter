using DataProcessor;
using DataProcessor.Interfaces;
using NUnit.Framework;
using NUnitTest;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TestDataProcessor.Parsors
{
    public class TestKMLParsor: TestBase
    {
        private IKMLParser parsor;
        private string pathToCorrect;
        private string pathToWrong;

        [SetUp]
        public void Setup()
        {
            parsor = new KMLParser();
        }

        [TestCase(@"Inputs\KML_Samples.kml")]
        public async Task TestWrongKML(string path)
        {
            var actual = await parsor.Parse(GetStreamFromPath(path));
            Assert.AreEqual(actual.Count(), 0);
        }

        [TestCase(@"Inputs\kml_test.kml")]
        public async Task TestRightKML(string path)
        {
            var actual = await parsor.Parse(GetStreamFromPath(path));
            Assert.AreEqual(actual.Count(), 1);
        }

        private Stream GetStreamFromPath(string path)
        {
            var filepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                path);
            return File.OpenRead(filepath);
        }
    }
}