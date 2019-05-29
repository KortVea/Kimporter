using Akavache;
using FreshMvvm;
using KimporterX.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace KimporterX
{
    public class LogPageModel : FreshBasePageModel
    {
        public LogPageModel()
        {
            ClearLogCommand = new Command(() =>
                BlobCache.UserAccount.InvalidateAllObjects<OperationHistory>()
                .Subscribe( i => LogList = new ObservableCollection<OperationHistory>()));
            BlobCache.UserAccount.GetAllObjects<OperationHistory>()
                .Subscribe( x => LogList = new ObservableCollection<OperationHistory>(x.OrderByDescending(i => i.Time)),
                    ex => LogList = new ObservableCollection<OperationHistory>());
        }

        public ObservableCollection<OperationHistory> LogList { get; set; } = new ObservableCollection<OperationHistory>();
        public ICommand ClearLogCommand { get; set; }
    }
}
