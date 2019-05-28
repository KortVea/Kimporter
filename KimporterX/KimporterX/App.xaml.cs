using Akavache;
using FreshMvvm;
using Xamarin.Forms;

namespace KimporterX
{
    public partial class App : Application
    {
        public const string JsonStrKey = "JsonStrKey";
        public const string ConnStrDic = "ConnStrDic";
        public App()
        {
            InitializeComponent();
            AppConfig.Config();

            var page = FreshPageModelResolver.ResolvePageModel<MainPageModel>();
            var basicNavContainer = new FreshNavigationContainer(page);
            MainPage = basicNavContainer;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            BlobCache.Shutdown().Wait();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
