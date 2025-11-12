using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using OMPS.ViewModels;
using OMPS.Windows;
using System;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using SCH = SQL_And_Config_Handler;

namespace OMPS.Pages
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {

        internal static Main_ViewModel MainViewModel { get => Ext.MainViewModel; }
        internal static MainWindow ParentWindow { get => Ext.MainWindow; }
        public Login()
        {
            InitializeComponent();

#if false
                // Image palette shift test
                var b = ColorPaletteHandler.ConvertImageFromUri(
                    "pack://application:,,,/OMPS;component/Images/fluent_web_dark_bg.png",
                    ColorPaletteHandler.Palettes.rct2
                );
                b.SaveToFile("C:\\test\\img.png");
#endif

            this.Txt_User.Text = UserName[0].ToString().ToUpper() + UserName[1..];
            this.Txt_User.IsReadOnly = true;
            this.Loaded += this.Login_Loaded;
        }

        private void Login_Loaded(object sender, RoutedEventArgs e)
        {
            this.Txt_Pass.Focus();
        }

        public static readonly string UserName = Environment.UserName;

        private void Btn_Next_Click(object sender, RoutedEventArgs e)
        {
            if (this.NextClickEnabled is false) return;
            DoChecks();
        }

        public bool NextClickEnabled = true;
        public SolidColorBrush DefaultBrush_IconForeg = new((Color)ColorConverter.ConvertFromString("#E5FFFFFF"));
        public SolidColorBrush DefaultBrush_BtnBorder = new((Color)ColorConverter.ConvertFromString("#FF1E90FF"));
        public SolidColorBrush DefaultBrush_TxtBorder = new((Color)ColorConverter.ConvertFromString("#B2FFFFFF"));
        public async void DoChecks()
        {
            this.Txt_User.IsEnabled = false;
            this.Txt_Pass.IsEnabled = false;
            this.Btn_Next.BeginAnimation(
                Button.HeightProperty,
                new DoubleAnimation(64, TimeSpan.FromSeconds(0.25))
                { EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } }
            );
            this.NextClickEnabled = false;
            this.Txt_Pass.BorderBrush = this.DefaultBrush_TxtBorder;
            this.Txtblk_Next.Visibility = Visibility.Collapsed;
            this.Progbar_Next.Value = 50;
            this.Progbar_Next.Visibility = Visibility.Visible;
            this.Spnl_Icons.Visibility = Visibility.Visible;
            await this.RunAllChecks();
            var cleared = CheckResults.Values.All(r => r is true);
            var failed = CheckResults.Where(p => p.Value is false).Select(p => p.Key);

            this.Btn_Next.BorderBrush = cleared ? Brushes.ForestGreen : Brushes.IndianRed;

            await Task.Delay(750);
            if (cleared)
            {
                this.Spnl_Icons.Visibility = Visibility.Collapsed;
                this.Txtblk_Next.Visibility = Visibility.Collapsed;
                var anim = new DoubleAnimation(32, TimeSpan.FromSeconds(0.25))
                { EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } };
                anim.Completed += this.NextBtn_ResetAnim_Completed;
                this.Btn_Next.BeginAnimation(
                    Button.HeightProperty, anim
                );
            } else
            {
                this.Progbar_Next.Visibility = Visibility.Collapsed;
                if (failed.Contains("UnPwValidation"))
                {
                    this.Txt_Pass.BorderBrush = Brushes.IndianRed;
                }
                await Task.Delay(1750);
                this.Spnl_Icons.Visibility = Visibility.Collapsed;
                this.Txtblk_Next.Visibility = Visibility.Visible;
                this.Btn_Next.BeginAnimation(
                    Button.HeightProperty,
                    new DoubleAnimation(32, TimeSpan.FromSeconds(0.25))
                    { EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut } }
                );
                this.Btn_Next.BorderBrush = this.DefaultBrush_BtnBorder;
                if (failed.Count() is 1 && failed.Contains("UnPwValidation"))
                {
                    this.ResetLoginBox(true);
                    this.Txt_Pass.Focus();
                    return;
                }
                MessageBox.Show($"The following required checks have failed, the application cannot load any further as a result of any of them failing.\n\n{string.Join(", ", failed)}");
            }
        }

        public void ResetLoginBox(bool skipPwInput = false)
        {
            this.Txt_Pass.IsEnabled = true;
            this.Txt_User.IsEnabled = true;
            this.Txtblk_Next.Visibility = Visibility.Visible;
            this.Progbar_Next.Visibility = Visibility.Collapsed;
            this.Spnl_Icons.Visibility = Visibility.Collapsed;
            foreach (var icon in Spnl_Icons.Children.OfType<PackIcon>())
            {
                if (icon.Name.EndsWith("Check"))
                {
                    icon.Opacity = 0.0;
                    icon.Foreground = this.DefaultBrush_IconForeg;
                } else
                {
                    icon.Opacity = 0.5;
                }
            }
            this.NextClickEnabled = true;
            if (skipPwInput is false)
            {
                this.Txt_Pass.BorderBrush = this.DefaultBrush_TxtBorder;
            }
            this.Btn_Next.BorderBrush = this.DefaultBrush_BtnBorder;
            this.Txt_Pass.Clear();
        }

        public static DoubleAnimation FadeOut<T>(T ele, DependencyProperty dep, double durationInSec, double delay = 0, EasingMode easeMode = EasingMode.EaseInOut) where T : FrameworkElement
        {
            var anim = new DoubleAnimation(ele.Opacity, 0, TimeSpan.FromSeconds(durationInSec))
            {
                BeginTime = TimeSpan.FromSeconds(delay),
                EasingFunction = new CubicEase() { EasingMode = easeMode }
            };
            ele.BeginAnimation(dep, anim);
            return anim;
        }

        private void NextBtn_ResetAnim_Completed(object? sender, EventArgs e)
        {
            if (ParentWindow is null) return;
            var storyboard = (Storyboard)this.Resources["LoginBox_Out"];
            Storyboard.SetTarget(storyboard, this.Border_LoginBox);
            storyboard.Completed += (ss, ee) =>
            {
                this.ResetLoginBox();
                this.Opacity = 0;
                var sb = new Storyboard()
                {
                    Children = [
                        new DoubleAnimation(ParentWindow.Width, 1025, TimeSpan.FromSeconds(0.5))
                        {
                            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut }
                        }.SetTarget(Window.WidthProperty, ParentWindow),
                        new DoubleAnimation(ParentWindow.Left, ParentWindow.Left - ((1025 - ParentWindow.Width) / 2.0), TimeSpan.FromSeconds(0.5))
                        {
                            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut }
                        }.SetTarget(Window.LeftProperty, ParentWindow)
                    ]
                };
                Storyboard.SetDesiredFrameRate(sb, 60);
                Storyboard.SetTarget(ParentWindow, sb);
                
                sb.Completed += (ss, ee) =>
                {
                    Ext.MainViewModel.CurrentPage = PageTypes.Home;
                    Ext.MainViewModel.WidgetMode = false;
                    this.Opacity = 1;
                };
                
                sb.Begin();
                //ParentWindow.BeginAnimation(Window.WidthProperty, anim);
                //ParentWindow.BeginAnimation(Window.LeftProperty, anim2);

                //
            };
            storyboard.Begin();
        }

        private static void CenterWindowOnScreen()
        {
            if (ParentWindow is null) return;
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = ParentWindow.Width;
            double windowHeight = ParentWindow.Height;
            ParentWindow?.Left = (screenWidth / 2) - (windowWidth / 2);
            ParentWindow?.Top = (screenHeight / 2) - (windowHeight / 2);
        }


        public readonly static Dictionary<string, bool> CheckResults = [];
        public async Task RunAllChecks()
        {
            CheckResults.Clear();
            // Login Info
            var un = this.Txt_User.Text;
            var pw = this.Txt_Pass.SecurePassword;
            await Task.Run(async () =>
            {
                await Task.Delay(250);
                //MessageBox.Show(string.Join(Environment.NewLine, [un, pw, Environment.UserDomainName, Environment.UserName, Environment.MachineName]));
                var res = false;
                res = ValidateUsernameAndPassword(un, pw);
                CheckResults.Add($"UnPwValidation", res);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.IcoPak_Acc.Opacity = 1;
                    this.IcoPak_AccCheck.Opacity = 1;
                    this.IcoPak_AccCheck.Foreground = !res ? Brushes.IndianRed : Brushes.ForestGreen;
                    this.IcoPak_AccCheck.Kind = !res ? PackIconKind.WarningDecagram : PackIconKind.CheckCircle;
                });

                //

                res = CheckDriveAccess();
                CheckResults.Add($"Network Drive Access\r\n\tRead access checks fails for one of the following locations: {string.Join(", ", NetworkLocations)}", res);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.IcoPak_Dir.Opacity = 1;
                    this.IcoPak_DirCheck.Opacity = 1;
                    this.IcoPak_DirCheck.Foreground = !res ? Brushes.IndianRed : Brushes.ForestGreen;
                    this.IcoPak_DirCheck.Kind = !res ? PackIconKind.WarningDecagram : PackIconKind.CheckCircle;
                });

                //

                res = await CheckConfig();
                CheckResults.Add($"Config File\r\n\t{SCH.Global.Config.GetErrorString()}", res);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.IcoPak_Config.Opacity = 1;
                    this.IcoPak_ConfigCheck.Opacity = 1;
                    this.IcoPak_ConfigCheck.Foreground = !res ? Brushes.IndianRed : Brushes.ForestGreen;
                    this.IcoPak_ConfigCheck.Kind = !res ? PackIconKind.WarningDecagram : PackIconKind.CheckCircle;
                });

                //

                res = await CheckDatabaseConnenctions();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.IcoPak_Db.Opacity = 1;
                    this.IcoPak_DbCheck.Opacity = 1;
                    this.IcoPak_DbCheck.Foreground = !res ? Brushes.IndianRed : Brushes.ForestGreen;
                    this.IcoPak_DbCheck.Kind = !res ? PackIconKind.WarningDecagram : PackIconKind.CheckCircle;
                });

                //

                res = CheckWebViewInstallation();
                CheckResults.Add("WebView2 installation\r\n\tCould not find Registry keys indicative of its installation on the machine", res);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.IcoPak_Webview2.Opacity = 1;
                    this.IcoPak_Webview2Check.Opacity = 1;
                    this.IcoPak_Webview2Check.Foreground = !res ? Brushes.IndianRed : Brushes.ForestGreen;
                    this.IcoPak_Webview2Check.Kind = !res ? PackIconKind.WarningDecagram : PackIconKind.CheckCircle;
                });

                Debug.WriteLine(string.Join(", ", CheckResults.Values));
            });
        }

        #region "Checks"
        /// <summary>
        ///     Validate username and password combination    
        ///     <para>Following Windows Services must be up</para>
        ///     <para>LanmanServer; TCP/IP NetBIOS Helper</para>
        /// </summary>
        /// <param name="userName">
        ///     Fully formatted UserName.
        ///     In AD: Domain + Username
        ///     In Workgroup: Username or Local computer name + Username
        /// </param>
        /// <param name="securePassword"></param>
        /// <returns></returns>
        public static bool ValidateUsernameAndPassword(string userName, SecureString securePassword)
        {
            bool result = false;

            ContextType contextType = ContextType.Machine;

            if (InDomain())
            {
                contextType = ContextType.Domain;
            }

            try
            {
                using PrincipalContext principalContext = new(contextType);
                result = principalContext.ValidateCredentials(
                    userName,
                    new NetworkCredential(string.Empty, securePassword).Password
                );
            }
            catch (PrincipalOperationException)
            {
                // Account disabled? Considering as Login failed
                result = false;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        /// <summary>
        ///     Validate: computer connected to domain?   
        /// </summary>
        /// <returns>
        ///     True -- computer is in domain
        ///     <para>False -- computer not in domain</para>
        /// </returns>
        public static bool InDomain()
        {
            bool result = true;

            try
            {
                Domain domain = Domain.GetComputerDomain();
            }
            catch (ActiveDirectoryObjectNotFoundException)
            {
                result = false;
            }

            return result;
        }

        public static readonly IEnumerable<string> NetworkLocations = ["P:", "L:", "H:", @"P:\~Dev", @"P:\!CRM", @"H:\Engineering", @"L:\Cutlists", @"I:\Update"];
        public static bool CheckDriveAccess()
        {
            //var user = WindowsIdentity.GetCurrent().User?.Value;
            //MessageBox.Show(user);
            return NetworkLocations.All(CanRead);
        }

        public static bool CanRead(string path)
        {
            try
            {
                var readAllow = false;
                var readDeny = false;
                var winuser = WindowsIdentity.GetCurrent().User;
                if (winuser is null) return false;
                var user = winuser.Value;
                //Debug.WriteLine(user);
                var accessControlList = new DirectoryInfo(path).GetAccessControl();
                if (accessControlList == null)
                    return false;

                //get the access rules that pertain to a valid SID/NTAccount.
                var accessRules = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
                if (accessRules == null)
                    return false;

                //we want to go over these rules to ensure a valid SID has access
                foreach (FileSystemAccessRule rule in accessRules)
                {
                    if ((FileSystemRights.Read & rule.FileSystemRights) != FileSystemRights.Read) continue;

                    if (rule.AccessControlType == AccessControlType.Allow)
                        readAllow = true;
                    else if (rule.AccessControlType == AccessControlType.Deny)
                        readDeny = true;
                    /*
                    Debug.WriteLine(rule.IdentityReference.Value);
                    if (!user.Equals(rule.IdentityReference.Value))
                    {
                        return false;
                    }
                    */
                }

                return readAllow && !readDeny;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        public static async Task<bool> CheckConfig()
        {
            if (SCH.Global.Config.IsConfigCompletelyLoaded)
            {
                return true;
            }
            else
            {
                var configInitRes = SCH.SQLDatabaseConnection.Init();
                return (configInitRes.Item1 is true && configInitRes.Item2 is null);
            }
        }

        public static async Task<bool> CheckDatabaseConnenctions()
        {
            bool[] execRes = new bool[2];
            var dbcon1 = new System.Data.Odbc.OdbcConnection($"Driver={{SQL Server}};Server={SCH.Global.Config["sql.servers.OldCRM"]};Database={SCH.Global.Config["sql.databases.OldCRM"]};DSN={SCH.Global.Config["sql.databases.OldCRM"]};Trusted_Connection=Yes;Integrated Security=SSPI;");
            var cmd1 = dbcon1.CreateCommand();
            cmd1.CommandText = "SELECT 1";
            cmd1.CommandTimeout = 10000;
            cmd1.Connection = dbcon1;
            await dbcon1.OpenAsync();
            execRes[0] = (await cmd1.ExecuteScalarAsync()) is int val1 && val1 is 1;
            await dbcon1.CloseAsync();

            CheckResults.Add($"Database - Old CRM\r\n\tFailed to connect or execute query on server '{SCH.Global.Config["sql.servers.OldCRM"]}' or database '{SCH.Global.Config["sql.databases.OldCRM"]}'", execRes[0]);

            // $"Driver={{SQL Server}};Data Source={Global.Config["sql.servers.DynamicsCRM"]};Integrated Security=SSPI;Persist Security Info=True;Packet Size=4096;Workstation ID=HPZ600-001;Initial Catalog={Global.Config["sql.databases.DynamicsCRM"]}"
            var dbcon2 = new System.Data.Odbc.OdbcConnection($"Driver={{SQL Server}};Server={SCH.Global.Config["sql.servers.DynamicsCRM"]};Database={SCH.Global.Config["sql.databases.DynamicsCRM"]};DSN={SCH.Global.Config["sql.databases.DynamicsCRM"]};Trusted_Connection=Yes;Integrated Security=SSPI;");

            var cmd2 = dbcon2.CreateCommand();
            cmd2.CommandText = "SELECT 1";
            cmd2.CommandTimeout = 10000;
            cmd2.Connection = dbcon2;
            await dbcon2.OpenAsync();
            execRes[1] = (await cmd2.ExecuteScalarAsync()) is int val2 && val2 is 1;
            await dbcon2.CloseAsync();

            CheckResults.Add($"Database - Dynamics CRM\r\n\tFailed to connect or execute query on server '{SCH.Global.Config["sql.servers.DynamicsCRM"]}' or database '{SCH.Global.Config["sql.databases.DynamicsCRM"]}'", execRes[1]);

            return execRes[0] && execRes[1] && true;
        }

        public static bool CheckWebViewInstallation()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                return Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}") is not null ||
                       Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}") is not null;
            }
            else
            {
                return Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}") is not null ||
                       Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}") is not null;
            }
        }
        #endregion


        private void Txt_Pass_TextInput(object sender, RoutedEventArgs e)
        {

            if (e.Source is PasswordBox txtb) 
            {
                if (((List<Regex>)[TwelveChars(), Uppercase(), Lowercase(), Numbers(), Symbols()]).All(reg => reg.IsMatch(txtb.Password)))
                {
                    this.Btn_Next.IsEnabled = true;
                    return;
                }
            }
            this.Btn_Next.IsEnabled = false;
        }
        private void Txt_Pass_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key is not System.Windows.Input.Key.Enter) return;
            if (this.Btn_Next.IsEnabled is false || this.NextClickEnabled is false) return;
            this.DoChecks();
        }

        [GeneratedRegex(".{12,}")]
        private static partial Regex TwelveChars();
        [GeneratedRegex("[A-Z]")]
        private static partial Regex Uppercase();
        [GeneratedRegex("[a-z]")]
        private static partial Regex Lowercase();
        [GeneratedRegex("[0-9]")]
        private static partial Regex Numbers();
        [GeneratedRegex("['\\-!\\\"#$%&\\(\\)\\*,\\./:;\\?@[\\]^_`\\{\\|\\}~\\+<=>]")]
        private static partial Regex Symbols();

    }

}
