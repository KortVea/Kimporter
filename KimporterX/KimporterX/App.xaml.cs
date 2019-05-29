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
            Registrations.Start("KimporterX");
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            //BlobCache.Shutdown().Wait(); //Not shutting down https://github.com/reactiveui/Akavache/issues/342
            BlobCache.UserAccount.Flush();
            BlobCache.Secure.Flush();

        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
