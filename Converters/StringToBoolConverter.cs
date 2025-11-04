using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace OMPS.Converters
{

    public class StringToBoolConverter : MarkupExtension, IValueConverter
    {
        public required string TrueValue { get; set; } = "true";
        public required string FalseValue { get; set; } = "false";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Note: this implementation precludes the use of "null" as the
            // value for TrueValue. Probably not an issue in 99.94% of all cases,
            // but something to consider, if one is looking to make a truly 100%
            // general-purpose class here.
            return value != null && EqualityComparer<string>.Default.Equals(((string)value).ToLower(), TrueValue.ToLower());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? FalseValue : ((bool)value ? TrueValue : FalseValue);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
