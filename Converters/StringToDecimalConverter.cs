using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace OMPS.Converters
{
    public class StringToDecimalConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return double.TryParse((string)value, out double _out) ? _out : 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToString((double)value);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
