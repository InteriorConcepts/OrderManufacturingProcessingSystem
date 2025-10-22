using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace OMPS.Converters
{
    public class SubtractConverter : MarkupExtension, IValueConverter
    {
        public double Value { get; set; }

        public object Convert(object baseValue, Type targetType, object parameter, CultureInfo culture)
        {
            double val = System.Convert.ToDouble(baseValue);
            return val - Value;
        }

        public object ConvertBack(object baseValue, Type targetType, object parameter, CultureInfo culture)
        {
            double val = System.Convert.ToDouble(baseValue);
            return val + Value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
