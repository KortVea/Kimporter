using FreshMvvm;
using Plugin.FilePicker;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace KimporterX
{
    public class MainPageModel : FreshBasePageModel
    {
        public MainPageModel()
        {
            IsManaging = false;
            ManageCommand = new Command(() => IsManaging = !IsManaging);
            OpenCommand = new FreshAwaitCommand(async (tcs) => {
                await GetKML();
                tcs.SetResult(true);
            });
            if (Application.Current.Properties.ContainsKey(App.JsonStrKey))
            {
                ConnStrJson = Application.Current.Properties[App.JsonStrKey] as string;
            }

        }

        private async Task GetKML()
        {
            try
            {
                var fileData = await CrossFilePicker.Current.PickFile();
                if (fileData == null) return;
                OpenButtonText = fileData.FilePath;
                var contents = Encoding.UTF8.GetString(fileData.DataArray);

            }
            catch (Exception ke)
            {
                await CoreMethods.DisplayAlert("Error", ke.Message, "OK");
                ResetControls();
            }

        }

        private void ResetControls()
        {
            OpenButtonText = "Open ...";
        }

        public ICommand ManageCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand SaveConfigCommand { get; set; }
        public ICommand ExecuteCommand { get; set; }
        public bool IsManaging { get; set; }
        public bool CanExecuteCommand => false;
        public string OpenButtonText { get; set; } = "Open ...";
        public string ConnStrJson { get; set; }
        public string ConnStrJsonPlaceHolder => "{ 'local db' : 'Server=localhost;Database=Tester;Trusted_Connection=True;', \n'Azure Test': '...', \n ... }";




    }
}
