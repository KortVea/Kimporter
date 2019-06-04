//using DataProcessor.DAL;
//using DataProcessor.Helpers;
//using DataProcessor.Interfaces;
//using DataProcessor.Models;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TestDAL
//{
//    [TestFixture]
//    public class TestRepo
//    {
//        private ITraceRepo<DownloadedTraceData> repo;
//        private List<DownloadedTraceData> list;

//        [SetUp]
//        public void Setup()
//        {
//            repo = new TraceRepo
//            {
//                ConnStr = @"Server=localhost;Database=EFProviders.InMemory;Trusted_Connection=True;ConnectRetryCount=0"//"Data Source=:memory:;Version=3;New=True;"
//            };
//            var g1 = Guid.NewGuid();
//            var g2 = Guid.NewGuid();
//            list = new List<DownloadedTraceData>
//            {
//                new DownloadedTraceData
//                {
//                    Id = g1,
//                    Type = 0,
//                    Time = DateTime.Now,
//                    Source = "SourceTest",
//                    Latitude = 0,
//                    Longitude = 1,
//                    Milage = 2,
//                    MilageSpecified = true,
//                    Heading = 90,
//                    HeadingSpecified = true,
//                    Speed = 3,
//                    SpeedSpecified = true,
//                    WasProcessed = false,
//                    NumberOfProperties = 1,
//                    OutOfSync = false
//                },
//                new DownloadedTraceData
//                {
//                    Id = g2,
//                    Type = 5,
//                    Time = DateTime.Now,
//                    Source = "SourceTest",
//                    Latitude = 2,
//                    Longitude = 3,
//                    Milage = 3,
//                    MilageSpecified = true,
//                    Heading = 180,
//                    HeadingSpecified = true,
//                    Speed = 4,
//                    SpeedSpecified = true,
//                    WasProcessed = false,
//                    NumberOfProperties = 1,
//                    OutOfSync = false
//                },
//            };
//            list[0].DownloadedPropertyData.Add(new DownloadedPropertyData { Id = Guid.NewGuid(), PropertyKey = "K1", PropertyValue = "V1", TraceDataId = g1 });
//            list[0].Hash = Hashing.CreateHash(list[0]);
//            list[1].DownloadedPropertyData.Add(new DownloadedPropertyData { Id = Guid.NewGuid(), PropertyKey = "K2", PropertyValue = "V2", TraceDataId = g2 });
//            list[1].Hash = Hashing.CreateHash(list[0]);
//        }

//        [Test]
//        public async Task TestInsertAllAsync()
//        {
//            var actual = await repo.InsertAllAsync(list);
//            Assert.AreEqual(actual, 2);

//        }


//    }
//}


////http://www.bradoncode.com/blog/2012/12/how-to-unit-test-repository.html
////to integration test
