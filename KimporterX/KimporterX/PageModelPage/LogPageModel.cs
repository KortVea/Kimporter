using Akavache;
using FreshMvvm;
using KimporterX.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace KimporterX
{
    public class LogPageModel : FreshBasePageModel
    {
        public LogPageModel()
        {
            BlobCache.UserAccount.GetAllObjects<OperationHistory>().Subscribe(
                x => LogList = new ObservableCollection<OperationHistory>(x.OrderByDescending(i => i.Time)),
                ex => LogList = new ObservableCollection<OperationHistory>()
                );
        }

        public ObservableCollection<OperationHistory> LogList { get; set; } = new ObservableCollection<OperationHistory>();
    }
}
