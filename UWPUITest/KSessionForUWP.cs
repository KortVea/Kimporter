using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using System;
using JsonConfig;
using System.Collections.Generic;

namespace UWPUITest
{
    public class KSessionForUWP
    {
        protected static WindowsDriver<WindowsElement> session;

        public static void SetUp(TestContext context)
        {
            if (session != null) return;
            var rawMap = new Dictionary<string, object>();
            foreach (var item in Config.Default.UWP)
            {
                rawMap.Add(item.Key, item.Value);
            }
            var appCapabilities = new DesiredCapabilities(rawMap);
            session = new WindowsDriver<WindowsElement>(new Uri(Config.Default.WindowsDriverUrl), appCapabilities);
            Assert.IsNotNull(session);

            session.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(Config.Default.Element_Search_Timeout));
        }

        public static void TearDown()
        {
            if (session == null) return;
            session.Quit();
            session = null;
        }
    }
}
