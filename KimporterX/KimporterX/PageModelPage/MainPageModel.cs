using DataProcessor;
using DataProcessor.DAL;
using DataProcessor.Models;
using FreshMvvm;
using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
            ResetControls();

            ManageCommand = new Command(() =>
            {
                IsManaging = !IsManaging;
                if (!IsManaging)
                {
                    HandleConnStr(ConnStrJson);
                }
            });

            OpenCommand = new FreshAwaitCommand(async (tcs) =>
            {
                await GetKML();
                tcs.SetResult(true);
            });

            SaveConfigCommand = new Command(HandleConnStr);

            ExecuteCommand = new Command(async () => await WritingKMLTraceAndProperties());

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
            stopWatch.Restart();
            IsBusy = true;
            abnormallyCount = 0;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                TimerText = stopWatch.Elapsed.ToString(@"hh\:mm\:ss");
                return IsBusy;
            });

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
                try
                {
                    var repo = new TraceRepo(connStrDictionary[SelectedConnStrKey]);
                    var endFiltered = dataToWrite.Max(i => i.Time);
                    await repo.InsertTracesAndPropsWhileIgnoringSameHash(dataToWrite,
                        new Progress<DbProgressInfo>(HandleDbProgressInfo),
                        end: endFiltered > DateTime.UtcNow ? DateTime.UtcNow : endFiltered);
                }
                catch (Exception de)
                {
                    await CoreMethods.DisplayAlert("Database", $"{de.Message}", "OK");
                    ResetControls();
                }
            }

            IsBusy = false;
            stopWatch.Stop();
            if (abnormallyCount > 0)
            {
                ExecuteButtonText += $"\nHash Collision Count: {abnormallyCount}";
            }
        }

        private void HandleDbProgressInfo(DbProgressInfo info)
        {
            switch (info.ProgressType)
            {
                case ProgressType.Default:
                    ExecuteButtonText = $"{info.Message}";
                    break;
                case ProgressType.Writing:
                    ExecuteButtonText = $"Writing {info.Number1} / {info.Number2}";
                    break;
                case ProgressType.Reading:
                    ExecuteButtonText = $"Reading {info.Number1} / {info.Number2}";
                    break;
                case ProgressType.Exception:
                    abnormallyCount++;
                    break;
                default:
                    break;
            }

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
            KMLInfoText = $"Life-sign trace count: {KMLLifeSignTraceData.Count()}\n" +
                $"Non-life-sign trace count: {KMLNonLifeSignTraceData.Count()}\n" +
                $"Total trace count: {kmlTraceData.Count()}\n" +
                $"Total attached trace property count: {kmlTraceData.SelectMany(i => i.DownloadedPropertyData).Count()}";
        }

        private void UpdateBindableProperties()
        {
            RaisePropertyChanged("KMLLifeSignTraceData");
            RaisePropertyChanged("KMLNonLifeSignTraceData");
            RaisePropertyChanged("ConnStrSource");
            RaisePropertyChanged("CanExecuteWriting");
        }

        private void ResetControls()
        {
            OpenButtonText = "Open ...";
            ExecuteButtonText = "Execute";
            TimerText = string.Empty;
        }

        private IEnumerable<DownloadedTraceData> kmlTraceData = new List<DownloadedTraceData>();
        private Dictionary<string, string> connStrDictionary = new Dictionary<string, string>();
        private Stopwatch stopWatch = new Stopwatch();
        private int abnormallyCount;

        public ObservableCollection<DownloadedTraceData> KMLLifeSignTraceData => new ObservableCollection<DownloadedTraceData>(kmlTraceData.Where(i => i.Type == 0).ToList());
        public ObservableCollection<DownloadedTraceData> KMLNonLifeSignTraceData => new ObservableCollection<DownloadedTraceData>(kmlTraceData.Where(i => i.Type != 0).ToList());
        public ObservableCollection<string> ConnStrSource => new ObservableCollection<string>(connStrDictionary.Keys);
        public string SelectedConnStrKey { get; set; }
        public ICommand ManageCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand SaveConfigCommand { get; set; }
        public ICommand ExecuteCommand { get; set; }
        public bool IsManaging { get; set; }
        public string OpenButtonText { get; set; }
        public string ExecuteButtonText { get; set; }
        public string ConnStrJson { get; set; }
        public string KMLInfoText { get; set; }
        public int SelectedTypeIndex { get; set; } = -1;
        public bool IsBusy { get; set; }
        public string TimerText { get; set; }
        public bool CanExecuteWriting => !IsBusy &&
            SelectedTypeIndex != -1 &&
            kmlTraceData?.Count() > 0 &&
            connStrDictionary.Count > 0 &&
            SelectedConnStrKey != null;
        public string ConnStrJsonPlaceHolder =>
@"//Put all your connection strings as Json object here. 
//The keys will be displayed in the dropdown list above.

{'Local Test' : 'Server=localhost;Database=Tester;Trusted_Connection=True;',
'Azure Test': '...', 
'Azure Production X': '...'}

//Troubleshooting:
//0. Privacy Setting - File System - this app is enabled
//1. DownloadedTraceData table and DownloadedPropertyData table are created according to their 
//schemas on your local SQL server.
//2. Protocols for SQLEXPRESS is installed in Computer Management brower, and its TCP/IP protoal 
is enabled
//3. SQL Server Brower is running in Service brower

//reference: https://docs.microsoft.com/en-us/windows/uwp/data-access/sql-server-databases#trouble-connecting-to-your-database";




    }
}
