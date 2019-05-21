using FreshMvvm;
using Plugin.FilePicker;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using DataProcessor;
using DataProcessor.Models;
using System.Collections.ObjectModel;

namespace KimporterX
{
    public class MainPageModel : FreshBasePageModel
    {
        public MainPageModel()
        {
            IsManaging = false;
            ManageCommand = new Command(() => IsManaging = !IsManaging);
            OpenCommand = new FreshAwaitCommand(async (tcs) => {
                //var test = await Repo.GetFullTrace();
                //var tes = Repo.GetAllTraces();
                await GetKML();
                
                tcs.SetResult(true);
            });
            if (Application.Current.Properties.ContainsKey(App.JsonStrKey))
            {
                ConnStrJson = Application.Current.Properties[App.JsonStrKey] as string;
            }

        }

        private async Task WritingKMLTraceAndProperties()
        {
            if (KMLTraceData.Count > 0)
            {

            }
        }

        private async Task GetKML()
        {
            try
            {
                var fileData = await CrossFilePicker.Current.PickFile(new string[] {".kml" });
                if (fileData == null) return;
                OpenButtonText = fileData.FilePath;
                //if (!fileData.FileName.EndsWith(".kml"))
                //{
                //    await CoreMethods.DisplayAlert("File Extension", "Please choose a .kml file.", "OK");
                //    return;
                //}
                KMLTraceData = new ObservableCollection<DownloadedTraceData>(KMLParsor.Parse(fileData.FilePath));

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


        public ObservableCollection<DownloadedTraceData> KMLTraceData { get; set; } = new ObservableCollection<DownloadedTraceData>();
        public ICommand ManageCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand SaveConfigCommand { get; set; }
        public ICommand ExecuteCommand { get; set; }
        public bool IsManaging { get; set; }
        public bool CanExecuteCommand => false;
        public string OpenButtonText { get; set; } = "Open ...";
        public string ConnStrJson { get; set; }
        public string ConnStrJsonPlaceHolder =>
@"//To connect to local SQL Server, make sure:
//0. DownloadedTraceData table and DownloadedPropertyData table are created according to their 
//schemas on your local SQL server.
//1. Protocols for SQLEXPRESS is installed in Computer Management brower, and its TCP/IP protoal 
is enabled
//2. SQL Server Brower is running in Service brower
//reference: https://docs.microsoft.com/en-us/windows/uwp/data-access/sql-server-databases
#trouble-connecting-to-your-database

//Put all your connections here. The keys will be displayed in the dropdown list above.
{ 'Local Test' : 'Server=localhost;Database=Tester;Trusted_Connection=True;',
'Azure Test': '...', 
'Azure Production X': '...' }";




    }
}
