using MaterialDesignThemes.Wpf;
using OMPS.viewModel;
using OMPS.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Windows.Devices.Enumeration;

namespace OMPS.Components
{
    /// <summary>
    /// Interaction logic for FeedbackToast.xaml
    /// </summary>
    public partial class FeedbackToast : UserControl, INotifyPropertyChanged, IDisposable
    {
        private readonly Storyboard? SBoard_In;
        private readonly Storyboard? SBoard_Out;
        private readonly Storyboard? SBoard_Prog;
        public readonly DispatcherTimer? ExpirationTimer;
        public FeedbackToast()
        {
            this.DataContext = this;
            this.Opacity = 0;
            InitializeComponent();
            //
            this.SBoard_In = this.Storyboard_Create_In();
            this.SBoard_Out = this.Storyboard_Create_Out();
            this.SBoard_Prog = this.Storyboard_Create_Prog();
            this.SBoard_Prog?.Duration = new TimeSpan(0, 0, 0, 0, ShownDurationMS);
            this.SBoard_In?.Completed += SBoard_In_Completed;
            this.SBoard_Out?.Completed += SBoard_Out_Completed;
            this.ExpirationTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, ShownDurationMS) };
            this.ExpirationTimer.Tick += ExpirationTimer_Tick;
            Ext.MainViewModel.PropertyChanged += MainViewModel_PropertyChanged;
        }

        public Storyboard Storyboard_Create_In()
        {
            var ease = EasingMode.EaseOut;
            var sb = new Storyboard() { };
            var a = new DoubleAnimationUsingKeyFrames()
            {
                KeyFrames = [
                    new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))) { EasingFunction = new CubicEase() { EasingMode = ease } },
                    new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(83))) { EasingFunction = new CubicEase() { EasingMode = ease } },
                    new EasingDoubleKeyFrame(0.95, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(500))) { EasingFunction = new CubicEase() { EasingMode = ease } },
                ]
            };
            var b = new DoubleAnimationUsingKeyFrames()
            {
                KeyFrames = [
                    new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))) { EasingFunction = new CubicEase() { EasingMode = ease } },
                    new EasingDoubleKeyFrame(FullHeight, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(500))) { EasingFunction = new CubicEase() { EasingMode = ease } },
                ]
            };
            var c = new ThicknessAnimationUsingKeyFrames()
            {
                KeyFrames = [
                    new EasingThicknessKeyFrame(new(0), KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))) { EasingFunction = new CubicEase() { EasingMode = ease } },
                    new EasingThicknessKeyFrame(new(0, 4, 0, 4), KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(500))) { EasingFunction = new CubicEase() { EasingMode = ease } },
                ]
            };
            Timeline.SetDesiredFrameRate(a, 120);
            Storyboard.SetTarget(a, this);
            Storyboard.SetTarget(a, this.RProgBar_Remaining);
            Storyboard.SetTargetProperty(a, new(FrameworkElement.OpacityProperty));
            sb.Children.Add(a);

            Timeline.SetDesiredFrameRate(b, 120);
            Storyboard.SetTarget(b, this);
            Storyboard.SetTargetProperty(b, new(FrameworkElement.HeightProperty));
            sb.Children.Add(b);

            Timeline.SetDesiredFrameRate(c, 120);
            Storyboard.SetTarget(c, this);
            Storyboard.SetTargetProperty(c, new(FrameworkElement.MarginProperty));
            sb.Children.Add(c);

            return sb;
        }

        public Storyboard Storyboard_Create_Out()
        {
            var ease = EasingMode.EaseInOut;
            var sb = new Storyboard() { };
            var a = new DoubleAnimationUsingKeyFrames()
            {
                KeyFrames = [
                    new EasingDoubleKeyFrame(0.95, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))) { EasingFunction = new CubicEase() { EasingMode = ease } },
                    new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300))) { EasingFunction = new CubicEase() { EasingMode = ease } },
                ]
            };
            var b = new DoubleAnimationUsingKeyFrames()
            {
                KeyFrames = [
                    new EasingDoubleKeyFrame(FullHeight, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(150))) { EasingFunction = new CubicEase() { EasingMode = ease } },
                    new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(550))) { EasingFunction = new CubicEase() { EasingMode = ease } },
                ]
            };
            var c = new ThicknessAnimationUsingKeyFrames()
            {
                KeyFrames = [
                    new EasingThicknessKeyFrame(new(0, 4, 0, 4), KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(150))) { EasingFunction = new CubicEase() { EasingMode = ease } },
                    new EasingThicknessKeyFrame(new(0), KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(550))) { EasingFunction = new CubicEase() { EasingMode = ease } },
                ]
            };
            Timeline.SetDesiredFrameRate(a, 120);
            Storyboard.SetTarget(a, this);
            Storyboard.SetTargetProperty(a, new(FrameworkElement.OpacityProperty));
            sb.Children.Add(a);

            Timeline.SetDesiredFrameRate(b, 120);
            Storyboard.SetTarget(b, this);
            Storyboard.SetTargetProperty(b, new(FrameworkElement.HeightProperty));
            sb.Children.Add(b);

            Timeline.SetDesiredFrameRate(c, 120);
            Storyboard.SetTarget(c, this);
            Storyboard.SetTargetProperty(c, new(FrameworkElement.MarginProperty));
            sb.Children.Add(c);

            return sb;
        }

        public Storyboard Storyboard_Create_Prog()
        {
            var sb = new Storyboard() { };
            var a = new DoubleAnimationUsingKeyFrames()
            {
                KeyFrames = [
                    new EasingDoubleKeyFrame(100, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))) { },
                    new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(ShownDurationMS))) { },
                ]
            };
            Timeline.SetDesiredFrameRate(a, 120);
            Storyboard.SetTarget(a, this.RProgBar_Remaining);
            Storyboard.SetTargetProperty(a, new(XamlRadialProgressBar.RadialProgressBar.ValueProperty));
            sb.Children.Add(a);

            return sb;
        }

        private void MainViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not string propName) return;
            if (propName is "FontSize_Base")
            {
                OnPropertyChanged(nameof(FullHeight));
                //this.OnPropertyChanged(e.PropertyName);
            }
        }

        public event EventHandler? ExitAnim_Completed;
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public static Main_ViewModel MainViewModel { get => Ext.MainViewModel; }

        public bool IsExpired = false;
        public bool IsExpanded = false;
        public bool IsHovered = false;

        public string Title
        {
            get => field;
            set
            {
                field = value;
                OnPropertyChanged(nameof(Title));
            }
        } = "Default Title";

        public string Body
        {
            get => field;
            set
            {
                field = value;
                OnPropertyChanged(nameof(Body));
            }
        } = "Default body contents, abc 123. Long overflowing description that will need to be expanded to read the entire thing.";

        public double OriginalFullHeight
        {
            get => field;
        } = Ext.MainViewModel.FontSize_H1 * 1.5;

        public double FullHeight
        {
            get => field;
            set
            {
                field = value;
                Debug.WriteLine(value);
                OnPropertyChanged(nameof(FullHeight));
            }
        } = Ext.MainViewModel.FontSize_H1 * 1.5;

        public double ExpandedHeight
        {
            get => field;
            set
            {
                field = value;
                OnPropertyChanged(nameof(ExpandedHeight));
            }
        } = Ext.MainViewModel.FontSize_H1 * 4.5;

        public int ShownDurationMS
        {
            get => field;
            set
            {
                field = value;
                OnPropertyChanged(nameof(ShownDurationMS));
            }
        } = 3300;

        public enum IconTypes
        {
            Info,
            Warn,
            Error,
        }

        public IconTypes IconType
        {
            get => field;
            set
            {
                field = value;
                OnPropertyChanged(nameof(IconType));
                OnPropertyChanged(nameof(TitleIconType));
                OnPropertyChanged(nameof(TitleIconColor));
            }
        }

        public PackIconKind TitleIconType
        {
            get => IconType switch
            {
                IconTypes.Info => PackIconKind.Info,
                IconTypes.Warn => PackIconKind.Warning,
                IconTypes.Error => PackIconKind.Error,
                _ => PackIconKind.Info,
            };
        }

        public SolidColorBrush TitleIconColor
        {
            get =>
                new(
                    IconType switch
                    {
                        IconTypes.Info => Color.FromRgb(131, 199, 226),
                        IconTypes.Warn => Color.FromRgb(255, 204, 119),
                        IconTypes.Error => Color.FromRgb(184, 45, 68),
                        _ => Colors.GhostWhite
                    }
                );
        }

        public void Show()
        {
            //this.BeginAnimation(UserControl.OpacityProperty, null);
            //this.BeginAnimation(UserControl.HeightProperty, null);
            //this.BeginAnimation(UserControl.MarginProperty, null);
            this.SBoard_In?.Begin();
        }
        public void Hide()
        {
            this.IsExpired = true;
            this.SBoard_Out?.Begin();
        }

        private void ExpirationTimer_Tick(object? sender, EventArgs e)
        {
            this.Hide();
        }

        private void SBoard_In_Completed(object? sender, EventArgs e)
        {
            this.BeginAnimation(UserControl.OpacityProperty, null);
            this.BeginAnimation(UserControl.HeightProperty, null);
            this.ExpirationTimer?.Start();
            this.SBoard_Prog?.Begin();
        }

        private void SBoard_Out_Completed(object? sender, EventArgs e)
        {
            this.ExitAnim_Completed?.Invoke(this, new EventArgs());
        }

        public void Pause()
        {
            this.SBoard_Prog?.Stop();
            this.RProgBar_Remaining.Value = 0;
            this.ExpirationTimer?.Stop();
            UpdateIcon();
        }

        public void UpdateIcon()
        {
            if (this.IsHovered)
            {
                this.PckIco_btn.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
            }
            else
            {
                if (IsExpanded)
                {
                    this.PckIco_btn.Kind = MaterialDesignThemes.Wpf.PackIconKind.ReadMore;
                } else
                {
                    this.PckIco_btn.Kind = MaterialDesignThemes.Wpf.PackIconKind.None;
                }
            }
        }

        public void Resume()
        {
            this.SBoard_Prog?.Begin();
            this.RProgBar_Remaining.Value = 100;
            this.ExpirationTimer?.Start();
            UpdateIcon();
        }

        private void ToastItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.IsExpired || this.IsExpanded) return;
            this.border.Background = new SolidColorBrush(Color.FromRgb(19, 19, 19));
            this.IsHovered = true;
            this.Pause();
        }

        private void ToastItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.IsExpired || this.IsExpanded) return;
            this.border.Background = new SolidColorBrush(Color.FromRgb(17, 17, 17));
            this.IsHovered = false;
            this.Resume();
        }

        private void Btn_Dismiss_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsExpanded)
            {
                Debug.WriteLine(message: "Show content");
                using (var dialog = new BodyPreviewWindow(Body) { Owner = Ext.MainWindow }) {
                    dialog.ShowDialog();
                }
            } else
            {
                Debug.WriteLine(message: "Force dismiss");
                this.ExpirationTimer?.Stop();
                this.Hide();
            }
        }

        private void border_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border b || b.Name is not "border" || e.OriginalSource is Button) return;
            if (this.IsExpired) return;
            Debug.WriteLine("Expand toggle");
            if (this.IsExpanded)
            {
                this.IsExpanded = false;
                this.IsHovered = true;
                UpdateIcon();
                this.FullHeight = this.OriginalFullHeight;
            } else
            {
                this.IsExpanded = true;
                this.IsHovered = false;
                this.Pause();
                this.FullHeight = this.ExpandedHeight;
            }
        }

        private void Btn_Dismiss_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.IsExpanded)
            {

            } else
            {
                this.PckIco_btn.Kind = MaterialDesignThemes.Wpf.PackIconKind.Close;
            }
        }

        private void Btn_Dismiss_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.IsExpanded)
            {

            }
            else
            {
                this.PckIco_btn.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
            }
        }

        public void Dispose()
        {
            this.ExpirationTimer?.Tick -= this.ExpirationTimer_Tick;
            this.ExpirationTimer?.Stop();
            this.SBoard_In?.Stop();
            this.SBoard_Out?.Stop();
            this.SBoard_Prog?.Stop();
            GC.SuppressFinalize(this);
        }
    }

    public class BodyPreviewWindow : Window, IDisposable
    {
        private Grid? _grid;
        private TextBox? _txt;
        public BodyPreviewWindow(string msg = "")
        {
            //
            this.Width = 500;
            this.Height = 500;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.Closing += BodyPreviewWindow_Closing;
            this.Background = new SolidColorBrush(Color.FromRgb(29, 29, 29));
            this.VerticalContentAlignment = VerticalAlignment.Stretch;
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            this.Title = "Toast Message Viewer";
            this._grid = new Grid() {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            this._grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            this._grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            this._txt = new TextBox()
            {
                Name = "txt_msg",
                Text = msg,
                IsReadOnly = true,
                IsReadOnlyCaretVisible = true,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Visible,
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                BorderBrush = Brushes.Gray,
                BorderThickness = new(1),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            Grid.SetColumn(this._txt, 0);
            Grid.SetRow(this._txt, 0);
            this._grid.Children.Add(this._txt);
            this.AddChild(this._grid);
            //
        }

        public void Dispose()
        {
            this.RemoveLogicalChild(this._txt);
            GC.SuppressFinalize(this);
        }

        private void BodyPreviewWindow_Closing(object? sender, CancelEventArgs e)
        {
            DialogResult = true;
        }
    }

}
