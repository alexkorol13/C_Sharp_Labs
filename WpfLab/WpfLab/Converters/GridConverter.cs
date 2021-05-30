using System;
using System.Windows.Data;
using Library;

namespace WpfLab.Converters
{
    public class GridConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            
            var gr = (V5DataCollection)value;
            return gr;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture){
            return value;
        }
    }
}