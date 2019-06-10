using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;

namespace UWPUITest
{
    [TestClass]
    public class ScenarioStandard : KSession
    {
        private static WindowsElement openBtn;
        private static WindowsElement manageBtn;
        private static WindowsElement historyBtn;
        private static WindowsElement typePicker;
        private static WindowsElement dataTypePicker;
        private static WindowsElement jsonEditor => session.FindElementByAccessibilityId("JsonStrEditor");
        private static WindowsElement validateBtn => session.FindElementByAccessibilityId("ValidateButton");

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
