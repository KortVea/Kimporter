using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace XFUITest
{
    [TestFixture(Platform.Android)]
    //[TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        [Test]
        public void TitleIsDisplayed()
        {
            AppResult[] results = app.WaitForElement(c => c.Marked("KML IMPORT"));
            app.Screenshot("Start Screen");

            Assert.IsTrue(results.Any());
        }

        [Test]
        public void ManageButtonWorks()
        {
            app.Tap(c => c.Marked("ManageButton"));
            var results = app.Query(c => c.Marked("JsonStrEditor"));

            Assert.IsTrue(results.Any());

            app.Tap(c => c.Marked("ManageButton"));
            results = app.Query(c => c.Marked("JsonStrEditor"));

            Assert.IsTrue(!results.Any());
        }

        [Test]
        public void ConnStrEditorWorks()
        {
            var expectedText = "{'a':'b', 'c':'d'}";
            app.Tap(c => c.Marked("ManageButton"));
            app.ClearText(c => c.Marked("JsonStrEditor"));
            app.EnterText(c => c.Marked("JsonStrEditor"), expectedText);
            app.Tap(c => c.Marked("ManageButton"));
            app.Tap(c => c.Marked("ManageButton"));

            var results = app.Query(c => c.Marked("JsonStrEditor"));

            Assert.AreEqual(expectedText, results?[0].Text);
        }

    }
}
