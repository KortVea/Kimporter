using JsonConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPUITest
{
    public class KSessionForAndroid
    {
        protected static AndroidDriver<AndroidElement> session;

        public static void SetUp(TestContext context)
        {
            Dictionary<string, object> rawMap = new Dictionary<string, object>();
            foreach (var item in Config.Default.Android)
            {
                rawMap.Add(item.Key, item.Value);
            }
            DesiredCapabilities appCapabilities = new DesiredCapabilities(rawMap);
            session = new AndroidDriver<AndroidElement>(new Uri(Config.Default.WindowsDriverUrl), appCapabilities);
            Assert.IsNotNull(session);

            session.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(Config.Default.Element_Search_Timeout));
        }

        public static void TearDown()
        {
            if (session != null)
            {
                session.Quit();
                session = null;
            }
        }
    }
}
