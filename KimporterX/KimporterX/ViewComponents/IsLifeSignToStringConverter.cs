using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace KimporterX.ViewComponents
{
    public class IsLifeSignToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (int)value;
            switch (type)
            {
                case 0: return "Life-sign";
                case 1: return "Non Life-sign";
                default: return "Both";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
