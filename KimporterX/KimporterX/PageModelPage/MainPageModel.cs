using Akavache;
using DataProcessor.Interfaces;
using DataProcessor.Models;
using FreshMvvm;
using KimporterX.Models;
using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace KimporterX
{
    public class MainPageModel : FreshBasePageModel
    {
        private ITraceRepo<DownloadedTraceData> _traceRepo;
        private IKMLParosr _kmlParsor;
        private IConnStrJsonParsor _connStrJsonParsor;
        public MainPageModel(ITraceRepo<DownloadedTraceData> traceRepo, IKMLParosr kMLParosr, IConnStrJsonParsor connStrJsonParsor)
        {
            _traceRepo = traceRepo;
            _kmlParsor = kMLParosr;
            _connStrJsonParsor = connStrJsonParsor;

            IsManaging = false;
            ResetControls();

            ManageCommand = new Command(async () =>
            {
                IsManaging = !IsManaging;
                if (!IsManaging)
                {
                    await HandleConnStr(ConnStrJson);
                }
            });

            OpenCommand = new FreshAwaitCommand(async (tcs) =>
            {
                await GetKML();
                tcs.SetResult(true);
            });

            OpenHistory = new FreshAwaitCommand(async (tcs) =>
            {
                await CoreMethods.PushPageModel<LogPageModel>();
                tcs.SetResult(true);
            });

            SaveConfigCommand = new Command(async (connStrJson) => await HandleConnStr(connStrJson));

            ExecuteCommand = new Command(async () => await WritingKMLTraceAndProperties());
        }

        public override void Init(object initData)
        {
            base.Init(initData);
            BlobCache.Secure.GetObject<string>(App.JsonStrKey)
                            .Subscribe(x => ConnStrJson = x, ex => ConnStrJson = "");

            BlobCache.Secure.GetObject<Dictionary<string, string>>(App.ConnStrDic)
                            .Subscribe(x =>
                            {
                                connStrDictionary = x;
                                RaisePropertyChanged("ConnStrSource");
                            },
                            ex => connStrDictionary = new Dictionary<string, string>());
        }

        private async Task HandleConnStr(object input)
        {
            try
            {
                var str = input as string;
                connStrDictionary = _connStrJsonParsor.Parse(str);
                await BlobCache.Secure.InsertObject(App.ConnStrDic, connStrDictionary, TimeSpan.FromDays(56));
                await BlobCache.Secure.InsertObject(App.JsonStrKey, ConnStrJson, TimeSpan.FromDays(56));
            }
            catch (Exception je)
            {
                await CoreMethods.DisplayAlert("Json Format", $"{je.Message}", "OK");
                connStrDictionary = new Dictionary<string, string>();
            }
            RaisePropertyChanged("ConnStrSource");
        }

        private async Task WritingKMLTraceAndProperties()
        {
            var shouldDisplayWarning = false;
            var msg = "";
            stopWatch.Restart();
            IsBusy = true;
            abnormallyCount = 0;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                TimerText = stopWatch.Elapsed.ToString(@"hh\:mm\:ss");
                return IsBusy;
            });

            switch (SelectedTypeIndex)
            {
                case 0: dataToWrite = KMLLifeSignTraceData; break;
                case 1: dataToWrite = KMLNonLifeSignTraceData; break;
                case 2: dataToWrite = kmlTraceData; break;
                default: return;
            }

            try
            {
                var connStr = connStrDictionary[SelectedConnStrKey];

                var endFiltered = dataToWrite.Max(i => i.Time);
                dataToWrite = await _traceRepo.InsertWithInMemoryCheck(dataToWrite,
                                                                new Progress<DbProgressInfo>(HandleDbProgressInfo),
                                                                endFiltered > DateTime.UtcNow ? DateTime.UtcNow : endFiltered,
                                                                connStr); //protect against abnormal data
                kmlTraceData = dataToWrite;
                UpdateCollectionBindableProperties();
            }
            catch (Exception de)
            {
                //to show the popup outside of try-catch, so that the stopwatch can stop ahead of popup.
                shouldDisplayWarning = true;
                msg = de.Message;
                ResetControls();
            }

            IsBusy = false;
            
            stopWatch.Stop();
            if (shouldDisplayWarning)
            {
                await CoreMethods.DisplayAlert("Database", $"{msg}", "OK");
            }
        }

        private async Task SaveToLog()
        {
            var hisKey = DateTime.Now.Ticks.GetHashCode().ToString();
            await BlobCache.UserAccount.InsertObject(hisKey, new OperationHistory
            {
                Time = DateTime.Now,
                FileName = fileName,
                ConnName = SelectedConnStrKey,
                Type = SelectedTypeIndex,
                Output = ExecuteButtonText.Replace("\n", "; ")
            });
        }

        private async void HandleDbProgressInfo(DbProgressInfo info)
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
                    ExecuteButtonText = $"Collision #{info.Number1}";
                    abnormallyCount++;
                    break;
                default:
                    break;
            }
            if (dataToWrite.Count() == 0 && !IsBusy)
            {
                ExecuteButtonText += "\nExecution ended";
                
            }
            if (abnormallyCount > 0 && !IsBusy)
            {
                ExecuteButtonText += $"\nHash collisions count: {abnormallyCount}";
            }
            if (!IsBusy)
            {
                await SaveToLog();
            }
        }

        private async Task GetKML()
        {
            try
            {
                var fileData = await CrossFilePicker.Current.PickFile(new string[] { ".kml" });
                if (fileData == null) return;
                OpenButtonText = fileData.FilePath;
                fileName = fileData.FileName;

                IsBusy = true;
                kmlTraceData = await _kmlParsor.Parse(fileData.GetStream()).ContinueWith(i => i.Result.ToList());
                IsBusy = false;

                if (kmlTraceData.Count() <= 0)
                {
                    await CoreMethods.DisplayAlert("File Contents", "No trace data were found from the file. Looking for Folder \"Trace points\" and Placemark tags.", "OK");
                }
            }
            catch (Exception ke)
            {
                await CoreMethods.DisplayAlert("Error", ke.Message, "OK");
                ResetControls();
            }

            UpdateAllBindableProperties();

            KMLInfoText = $"Life-sign trace count: {KMLLifeSignTraceData.Count()}\n" +
            $"Non-life-sign trace count: {KMLNonLifeSignTraceData.Count()}\n" +
            $"Total trace count: {kmlTraceData.Count()}\n" +
            $"Total attached trace property count: {kmlTraceData.SelectMany(i => i.DownloadedPropertyData).Count()}";
            ExecuteButtonText = "Execute";
        }

        private void UpdateCollectionBindableProperties()
        {
            RaisePropertyChanged("KMLLifeSignTraceData");
            RaisePropertyChanged("KMLNonLifeSignTraceData");
        }

        private void UpdateAllBindableProperties()
        {
            RaisePropertyChanged("KMLLifeSignTraceData");
            RaisePropertyChanged("KMLNonLifeSignTraceData");
            RaisePropertyChanged("CanExecuteWriting");
        }

        private void ResetControls()
        {
            OpenButtonText = "Open ...";
            ExecuteButtonText = "Execute";
            TimerText = string.Empty;
        }

        private IEnumerable<DownloadedTraceData> kmlTraceData = new List<DownloadedTraceData>();
        private IEnumerable<DownloadedTraceData> dataToWrite = new List<DownloadedTraceData>();
        private Dictionary<string, string> connStrDictionary = new Dictionary<string, string>();
        private Stopwatch stopWatch = new Stopwatch();
        private int abnormallyCount;
        private string fileName = "";

        public ObservableCollection<DownloadedTraceData> KMLLifeSignTraceData => new ObservableCollection<DownloadedTraceData>(kmlTraceData.Where(i => i.Type == 0).ToList());
        public ObservableCollection<DownloadedTraceData> KMLNonLifeSignTraceData => new ObservableCollection<DownloadedTraceData>(kmlTraceData.Where(i => i.Type != 0).ToList());
        public ObservableCollection<string> ConnStrSource => new ObservableCollection<string>(connStrDictionary.Keys);
        public string SelectedConnStrKey { get; set; }
        public ICommand ManageCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand SaveConfigCommand { get; set; }
        public ICommand ExecuteCommand { get; set; }
        public ICommand OpenHistory { get; set; }
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
