using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPUITest
{
    [TestClass]
    public class ScenarioStandardForAndroid : KSessionForAndroid
    {
        private static AndroidElement openBtn;
        private static AndroidElement manageBtn;
        private static AndroidElement historyBtn;
        private static AndroidElement typePicker;
        private static AndroidElement dataTypePicker;
        private static AndroidElement jsonEditor => session.FindElementByAccessibilityId("JsonStrEditor");
        private static AndroidElement validateBtn => session.FindElementByAccessibilityId("ValidateButton");

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            SetUp(context);
            try
            {
                openBtn = session.FindElementByAccessibilityId("OpenButton");
                Assert.AreEqual("Open ...", openBtn.Text);
                manageBtn = session.FindElementByAccessibilityId("ManageButton");
                Assert.IsNotNull(manageBtn);
                historyBtn = session.FindElementByAccessibilityId("HistoryButton");
                Assert.IsNotNull(historyBtn);
                typePicker = session.FindElementByAccessibilityId("ConnStrPicker");
                Assert.IsNotNull(typePicker);
                dataTypePicker = session.FindElementByAccessibilityId("DataTypePicker");
                Assert.IsNotNull(dataTypePicker);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TearDown();
        }

        [TestMethod]
        public void HistoryButtonShouldNavigate()
        {
            historyBtn.Click();
            var actual = session.FindElementsByAccessibilityId("ClearLogButton").Count();
            Assert.AreEqual(1, actual);
            session.FindElementByName("Back").Click();

        }

        [TestMethod]
        public void OpenButtonShouldOpenDialog()
        {
            openBtn.Click();
            session.FindElementByName("Cancel").Click();
            Assert.AreEqual(0, session.FindElementsByName("Cancel").Count);
        }

        [TestMethod]
        public void ManageButtonShouldOpenEditorAndConfigWorks()
        {
            var expected = "{'a':'b', 'c': 'd'}";
            manageBtn.Click();
            Assert.AreEqual(1, session.FindElementsByAccessibilityId("JsonStrEditor").Count());

            jsonEditor.Clear();
            jsonEditor.SendKeys("{'a':'b', ");
            validateBtn.Click();
            Assert.AreEqual(1, session.FindElementsByName("OK").Count);
            session.FindElementByName("OK").Click();

            jsonEditor.Clear();
            jsonEditor.SendKeys(expected);
            validateBtn.Click();
            Assert.AreEqual(0, session.FindElementsByName("OK").Count);

            manageBtn.Click();
            manageBtn.Click();
            var actual = jsonEditor.Text;
            Assert.AreEqual(expected, actual);

            typePicker.SendKeys("c");
            Assert.AreEqual("c", typePicker.Text);
        }

        [TestMethod]
        public void TypePickerShouldWork()
        {
            var expected = "Both";
            dataTypePicker.SendKeys(expected);
            Assert.AreEqual(expected, dataTypePicker.Text);
        }
    }
}
