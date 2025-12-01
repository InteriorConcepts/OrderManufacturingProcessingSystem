using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace OMPS.Converters
{
    public class StringToWorkCtrCustom : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return "1";
            if (Ext.WorkCtrValues.FirstOrDefault(w => w.Value.ToString() == value.ToString()) is not Ext.CustomComboBoxDisplayValues obj) return "1";
            Debug.WriteLine($"!! Converter: {value} ({obj.DisplayName})");
            return obj.DisplayName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return "2";
            if (Ext.WorkCtrValues.FirstOrDefault(w => w.DisplayName == value.ToString()) is not Ext.CustomComboBoxDisplayValues obj) return "2";
            Debug.WriteLine($"!! Converter Back: {value} ({obj.Value})");
            return obj.Value;
        }

        public static string Convert(object key)
        {
            if (Ext.WorkCtrValues.FirstOrDefault(w => w.Value.ToString() == key.ToString()) is not Ext.CustomComboBoxDisplayValues obj) return "1";
            Debug.WriteLine($"!! Converter: {key} ({obj.DisplayName})");
            return obj.DisplayName;
        }

        public static object Convert(string key)
        {
            if (Ext.WorkCtrValues.FirstOrDefault(w => w.DisplayName == key.ToString()) is not Ext.CustomComboBoxDisplayValues obj) return "2";
            Debug.WriteLine($"!! Converter Back: {key} ({obj.Value})");
            return obj.Value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
