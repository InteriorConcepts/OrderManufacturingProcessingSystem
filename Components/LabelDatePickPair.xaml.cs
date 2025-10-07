using System;
using System.Collections.Generic;
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
    /// Interaction logic for LabelDatePickPair.xaml
    /// </summary>
    public partial class LabelDatePickPair : UserControl
    {
        public LabelDatePickPair()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(LabelDatePickPair),
                new PropertyMetadata("Label:"));

        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof(int), typeof(LabelDatePickPair),
                new PropertyMetadata(65));

        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register("InputText", typeof(string), typeof(LabelDatePickPair),
                new PropertyMetadata("DefaultValue"));

        public static readonly DependencyProperty InputFormatProperty =
            DependencyProperty.Register("InputFormat", typeof(string), typeof(LabelDatePickPair),
                new PropertyMetadata("{}"));

        public static readonly DependencyProperty InputReadOnlyProperty =
            DependencyProperty.Register("InputReadOnly", typeof(bool), typeof(LabelDatePickPair),
                new PropertyMetadata(false));

        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(DateTime), typeof(LabelDatePickPair),
                new PropertyMetadata(DateTime.Today));

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public int LabelWidth
        {
            get { return (int)GetValue(LabelWidthProperty); }
            set { SetValue(LabelWidthProperty, value); }
        }

        public string InputText
        {
            get { return (string)GetValue(InputTextProperty); }
            set { SetValue(InputTextProperty, value); }
        }

        public DateTime? SelectedDate
        {
            get { return (DateTime)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        public bool InputReadOnly
        {
            get { return (bool)GetValue(InputReadOnlyProperty); }
            set { SetValue(InputReadOnlyProperty, value); }
        }

        public bool InputEnabled
        {
            get { return !(bool)GetValue(InputReadOnlyProperty); }
        }
    }
}
