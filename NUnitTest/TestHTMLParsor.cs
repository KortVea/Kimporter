using NUnit.Framework;
using DataProcessor;
using HtmlAgilityPack;
using NUnitTest;
using DataProcessor.Models;

namespace TestDataProcessor.Parsors
{
    [TestFixture]
    public class TestHTMLParsor : TestBase
    {
        private string htmlToTest;

        [SetUp]
        public void Setup()
        {
            htmlToTest = @"<table style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; ""><tr style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; ""><th style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">type</th><td style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">19</td></tr><tr style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; ""><th style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">Unit</th><td style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">BM222</td></tr><tr style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; ""><th style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">Date</th><td style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">18/07/18 06:50:11</td></tr><tr style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; ""><th style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">Location</th><td style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">Munjina-Roy Hill Rd, 6753 Newman, Western Australia, AU</td></tr><tr style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; ""><th style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">Speed(mph)</th><td style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">7.5</td></tr><tr style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; ""><th style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">Mileage(mi)</th><td style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">82503.8</td></tr><tr style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; ""><th style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">Orientation</th><td style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">West</td></tr><tr style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; ""><th style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">Activity duration</th><td style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">41s</td></tr><tr style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; ""><th style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">IO type</th><td style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">PTO Switch</td></tr><tr style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; ""><th style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">Started</th><td style=""border : 1px solid blue; border-collapse: collapse; text-align: left; padding: 3px; "">18/07/18 08:49</td></tr></table>";
        }

        [Test]
        public void Test1()
        {
            var actual = HTMLStringParsor.Parse(htmlToTest);
            
            Assert.AreEqual(actual.DownloadedPropertyData.Count, 3);
            Assert.AreEqual(actual.Type, 19);
            Assert.AreEqual(actual.Speed, 7.5);
            Assert.IsTrue(actual.DownloadedPropertyData.ContainsKey("IO type"));
        }

    }
}
