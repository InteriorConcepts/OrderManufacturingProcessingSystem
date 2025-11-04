using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OMPS.Components
{
    /// <summary>
    /// Interaction logic for LabelInputLookupPair.xaml
    /// </summary>
    public partial class LabelInputLookupPair : UserControl
    {
        public LabelInputLookupPair()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(LabelInputLookupPair),
                new PropertyMetadata("Label:"));

        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof(int), typeof(LabelInputLookupPair),
                new PropertyMetadata(65));

        public static readonly DependencyProperty InputLookupValueProperty =
            DependencyProperty.Register("InputLookupValue", typeof(string), typeof(LabelInputLookupPair),
                new PropertyMetadata("-"));

        public static readonly DependencyProperty InputLookupProperty =
            DependencyProperty.Register("InputLookup", typeof(string), typeof(LabelInputLookupPair),
                new PropertyMetadata("DefaultValue", OnLookupChanged));

        public static readonly DependencyProperty InputFormatProperty =
            DependencyProperty.Register("InputFormat", typeof(string), typeof(LabelInputLookupPair),
                new PropertyMetadata("{}"));

        public static readonly DependencyProperty LookupEnabledProperty =
            DependencyProperty.Register("LookupEnabled", typeof(bool), typeof(LabelInputLookupPair),
                new PropertyMetadata(true));

        public static readonly DependencyProperty InputLookupTypeProperty =
            DependencyProperty.Register("InputLookupType", typeof(string), typeof(LabelInputLookupPair),
                new PropertyMetadata(""));

        public static readonly DependencyProperty ConvPairProperty =
            DependencyProperty.Register("ConvPair", typeof(Func<LabelInputLookupPair, string>), typeof(LabelInputLookupPair),
                new PropertyMetadata(null));

        private static void OnLookupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not LabelInputLookupPair lookupEle) return;
            lookupEle.InputLookupChanged?.Invoke(lookupEle, CreateEventArgsObj(lookupEle));
        }

        public class Lookup_EventArgs : EventArgs
        {
            public required string LookupType { get; set; }
            public required string Lookup { get; set; }
            public required string LookupValue { get; set; }
            public required LabelInputLookupPair Source { get; set; }
        }

        public event EventHandler<Lookup_EventArgs>? LookupButtonPressed;

        public event EventHandler<Lookup_EventArgs>? InputLookupChanged;

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public int LabelFullWidth
        {
            get { return (int)GetValue(LabelWidthProperty); }
            set { SetValue(LabelWidthProperty, value - 16); }
        }

        public int LabelWidth
        {
            get { return (int)GetValue(LabelWidthProperty) - 16; }
            set { SetValue(LabelWidthProperty, value); }
        }

        public Func<LabelInputLookupPair, string> ConvFunc {
            get => (Func<LabelInputLookupPair, string>)GetValue(ConvPairProperty);
            set => SetValue(ConvPairProperty, value);
        }

        public string InputLookup
        {
            get { return (string)GetValue(InputLookupProperty); }
            set {
                if (value is null) return;
                if (InputLookup == value) return;
                Debug.WriteLine("**********");
                SetValue(InputLookupProperty, value);
                this.InputLookupValue = this.ConvFunc?.Invoke(this) ?? "";
                this.InputLookupChanged?.Invoke(this, CreateEventArgsObj(this));
            }
        }

        public string InputLookupType
        {
            get { return (string)GetValue(InputLookupTypeProperty); }
            set { SetValue(InputLookupTypeProperty, value); }
        }

        public string InputLookupValue
        {
            get { return (string)GetValue(InputLookupValueProperty); }
            set { SetValue(InputLookupValueProperty, value); }
        }

        public bool LookupEnabled
        {
            get { return (bool)GetValue(LookupEnabledProperty); }
            set { SetValue(LookupEnabledProperty, value); }
        }

        private static Lookup_EventArgs CreateEventArgsObj(LabelInputLookupPair ele) =>
            new()
            {
                LookupType = ele.InputLookupType,
                LookupValue = ele.InputLookupValue,
                Lookup = ele.InputLookup,
                Source = ele
            };

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!this.LookupEnabled) return;
            this.LookupButtonPressed?.Invoke(this, CreateEventArgsObj(this));
        }
    }
}
