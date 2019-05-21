using FreshMvvm;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using DataProcessor;
using DataProcessor.Models;
using System.Collections.ObjectModel;
using System.Linq;
using Plugin.FilePicker;
using System.Collections.Generic;

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

        private async Task WritingKMLTraceAndProperties()
        {
            if (kmlTraceData.Count() > 0)
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
                kmlTraceData = KMLParsor.Parse(fileData.GetStream());
                if (kmlTraceData == null || kmlTraceData.Count() <= 0)
                {
                    await CoreMethods.DisplayAlert("File Contents", "No trace data were found from the file. Looking for Folder \"Trace points\" and Placemark tags.", "OK");
                }
                UpdateBindableProperties();

            }
            catch (Exception ke)
            {
                await CoreMethods.DisplayAlert("Error", ke.Message, "OK");
                ResetControls();
            }

        }

        private void UpdateBindableProperties()
        {
            RaisePropertyChanged("KMLLifeSignTraceData");
            RaisePropertyChanged("KMLNonLifeSignTraceData");
        }

        private void ResetControls()
        {
            OpenButtonText = "Open ...";
        }


        private IEnumerable<DownloadedTraceData> kmlTraceData = new List<DownloadedTraceData>();
        public ObservableCollection<DownloadedTraceData> KMLLifeSignTraceData => new ObservableCollection<DownloadedTraceData>(kmlTraceData.Where(i => i.Type == 0).ToList());
        public ObservableCollection<DownloadedTraceData> KMLNonLifeSignTraceData => new ObservableCollection<DownloadedTraceData>(kmlTraceData.Where(i => i.Type != 0).ToList());
        public ICommand ManageCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand SaveConfigCommand { get; set; }
        public ICommand ExecuteCommand { get; set; }
        public bool IsManaging { get; set; }
        public bool CanExecuteCommand => false;
        public string OpenButtonText { get; set; } = "Open ...";
        public string ConnStrJson { get; set; }
        public string ConnStrJsonPlaceHolder =>
@"//Put all your connections here. The keys will be displayed in the dropdown list above.
{'Local Test' : 'Server=localhost;Database=Tester;Trusted_Connection=True;',
'Azure Test': '...', 
'Azure Production X': '...'}

//To connect to local SQL Server, make sure:
//0. DownloadedTraceData table and DownloadedPropertyData table are created according to their 
//schemas on your local SQL server.
//1. Protocols for SQLEXPRESS is installed in Computer Management brower, and its TCP/IP protoal 
is enabled
//2. SQL Server Brower is running in Service brower
//reference: https://docs.microsoft.com/en-us/windows/uwp/data-access/sql-server-databases
#trouble-connecting-to-your-database";




    }
}
