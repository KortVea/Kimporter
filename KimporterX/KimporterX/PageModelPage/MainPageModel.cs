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
using DataProcessor.DAL;

namespace KimporterX
{
    public class MainPageModel : FreshBasePageModel
    {
        public MainPageModel()
        {
            IsManaging = false;
            ManageCommand = new Command(() => IsManaging = !IsManaging);

            OpenCommand = new FreshAwaitCommand(async (tcs) =>
            {
                await GetKML();
                tcs.SetResult(true);
            });

            SaveConfigCommand = new Command(HandleConnStr);

            ExecuteCommand = new Command(async ()=> await WritingKMLTraceAndProperties());

            if (Application.Current.Properties.ContainsKey(App.JsonStrKey))
            {
                ConnStrJson = Application.Current.Properties[App.JsonStrKey] as string;
                HandleConnStr(ConnStrJson);
            }

        }

        private void HandleConnStr(object input)
        {
            try
            {
                var str = input as string;
                connStrDictionary = ConnStrJsonParsor.Parse(str);
                Application.Current.Properties[App.JsonStrKey] = str;
            }
            catch (Exception je)
            {
                CoreMethods.DisplayAlert("Json Format", $"{je.Message}", "OK");
                connStrDictionary = new Dictionary<string, string>();
            }
            UpdateBindableProperties();
        }

        private async Task WritingKMLTraceAndProperties()
        {
            IsBusy = true;
            IEnumerable<DownloadedTraceData> dataToWrite;
            switch (SelectedTypeIndex)
            {
                case 0: dataToWrite = KMLLifeSignTraceData; break;
                case 1: dataToWrite = KMLNonLifeSignTraceData; break;
                case 2: dataToWrite = kmlTraceData; break;
                default: return;
            }
            var totalCount = dataToWrite.Count();
            if (totalCount > 0)
            {
                var repo = new TraceRepo(connStrDictionary[SelectedConnStrKey]);
                await repo.InsertTracesAndPropsWhileIgnoringSameHash(dataToWrite, 
                    new Progress<int>((count) => {
                        ExecuteButtonText = $"{count} / {totalCount}";
                    }));
            }
            IsBusy = false;
        }

        private async Task GetKML()
        {
            try
            {
                var fileData = await CrossFilePicker.Current.PickFile(new string[] { ".kml" });
                if (fileData == null) return;
                OpenButtonText = fileData.FilePath;
                kmlTraceData = KMLParsor.Parse(fileData.GetStream());
                if (kmlTraceData == null || kmlTraceData.Count() <= 0)
                {
                    await CoreMethods.DisplayAlert("File Contents", "No trace data were found from the file. Looking for Folder \"Trace points\" and Placemark tags.", "OK");
                }
            }
            catch (Exception ke)
            {
                await CoreMethods.DisplayAlert("Error", ke.Message, "OK");
                ResetControls();
            }
            UpdateBindableProperties();

        }

        private void UpdateBindableProperties()
        {
            RaisePropertyChanged("KMLLifeSignTraceData");
            RaisePropertyChanged("KMLNonLifeSignTraceData");
            RaisePropertyChanged("ConnStrSource");
        }

        private void ResetControls()
        {
            OpenButtonText = "Open ...";
        }

        private IEnumerable<DownloadedTraceData> kmlTraceData = new List<DownloadedTraceData>();
        private Dictionary<string, string> connStrDictionary = new Dictionary<string, string>();

        public ObservableCollection<DownloadedTraceData> KMLLifeSignTraceData => new ObservableCollection<DownloadedTraceData>(kmlTraceData.Where(i => i.Type == 0).ToList());
        public ObservableCollection<DownloadedTraceData> KMLNonLifeSignTraceData => new ObservableCollection<DownloadedTraceData>(kmlTraceData.Where(i => i.Type != 0).ToList());
        public ObservableCollection<string> ConnStrSource => new ObservableCollection<string>(connStrDictionary.Keys);
        public string SelectedConnStrKey { get; set; }
        public ICommand ManageCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand SaveConfigCommand { get; set; }
        public ICommand ExecuteCommand { get; set; }
        public bool IsManaging { get; set; }
        public bool CanExecuteWriting => false;
        public string OpenButtonText { get; set; } = "Open ...";
        public string ExecuteButtonText { get; set; } = "Execute";
        public string ConnStrJson { get; set; }
        public int SelectedTypeIndex { get; set; } = -1;
        public bool IsBusy { get; set; }
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
