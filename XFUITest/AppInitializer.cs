using System;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace XFUITest
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp.Android.InstalledApp("com.sitech.KimporterX").StartApp();
            }

            return ConfigureApp.iOS.StartApp();
        }
    }
}