using KimporterX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KimporterX.ViewComponents
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogViewCell : ViewCell
    {
        public LogViewCell()
        {
            InitializeComponent();
        }

        //public static readonly BindableProperty OperationHistoryProperty = BindableProperty.Create("OperationHistory", typeof(OperationHistory), typeof(LogViewCell),
        //    propertyChanged: OperationHistoryChanged);

        //public OperationHistory OperationHistory
        //{
        //    get { return (OperationHistory)GetValue(OperationHistoryProperty); }
        //    set { SetValue(OperationHistoryProperty, value); }
        //}

        //private static void OperationHistoryChanged(BindableObject bindable, object oldValue, object newValue)
        //{
        //    var cell = (LogViewCell)bindable;
        //    var log = (OperationHistory)newValue;
        //    cell.time.Text = log.Time.ToShortDateString();
        //    cell.fileName.Text = log.FileName;
        //    cell.connName.Text = log.ConnName;
        //    cell.isLifeSign.Text = log.IsLifeSign ? "Life-sign" : "Non-life-sign";
        //    cell.isLifeSign.TextColor = log.IsLifeSign ? Color.Red : Color.Blue;
        //}
    }
}