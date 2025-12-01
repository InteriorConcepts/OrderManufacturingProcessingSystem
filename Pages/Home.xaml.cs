using Azure;
using CommunityToolkit.Mvvm.DependencyInjection;
using Humanizer;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using OMPS.DBModels;
using OMPS.ViewModels;
using OMPS.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using static OMPS.Pages.Home;

namespace OMPS.Pages
{

    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl, INotifyPropertyChanged
    {
        public byte firstLoad = 0;
        public Home()
        {
            InitializeComponent();
            //
            this.DataContext = this;
            //
            this.LoadShortcutsFromSettings();
            this.Refresher.Elapsed += Refresher_Elapsed;
            this.Refresher.Start();
        }

        public uint infrequency = 0;
        public uint infrequentAltInterval = 1;
        public void LoadData()
        {
            new Task(
                () => {
                    Debug.WriteLine("Home update");
                    this.GetLocalFolders();
                    Debug.WriteLine("GetLocalFolders");
                    this.GetEngChecks();
                    Debug.WriteLine("GetEngChecks");
                    this.GetEngReleases();
                    Debug.WriteLine("GetEngReleases");
                    this.GetCncWorking();
                    Debug.WriteLine("GetCncWorking");
                },
                TaskCreationOptions.DenyChildAttach
            ).Start();
            if (infrequency is 0 || infrequency == infrequentAltInterval)
            {
                Debug.WriteLine("Infrequent home update");
                Dispatcher.BeginInvoke(() =>
                {
                    this.GetNewOrders();
                    Debug.WriteLine("GetNewOrders");
                    this.GetEngWorking();
                    Debug.WriteLine("GetEngWorking");
                    this.GetEngArchive();
                    Debug.WriteLine("GetEngArchive");
                }, System.Windows.Threading.DispatcherPriority.Loaded);
                infrequency = 0;
            }
            infrequency++;
        }

        public void LoadShortcutsFromSettings()
        {
            if (Ext.ReadSetting<string>(Ext.AppConfigKey.Shortcuts) is not (bool, string) res || !res.success || string.IsNullOrWhiteSpace(res.value)) return;
            var split = res.value.Split(';');
            if (split is null || split.Length is 0) return;
            if (TryFindResource("Home_ShortcutButton") is not object obj || obj is not ControlTemplate ct) return;
            foreach (var shortcut in split)
            {
                if (shortcut.Split('+') is not string[] infoSplit || infoSplit is null || (infoSplit.Length is not 3 && infoSplit.Length is not 4)) continue;
                (string type, string name, string dest, string img) = (infoSplit[0].ToLower(), infoSplit[1], infoSplit[2], infoSplit[3]);
                FrameworkElement? ico = null;
                //MessageBox.Show(String.Join("\n", infoSplit));
                if (img.Length is not 0)
                {
                    if (img.StartsWith("/"))
                    {
                        ico = new Image() { };
                        ((Image)ico).Source = new BitmapImage(new Uri(
                            @"pack://application:,,,/OMPS;component" +
                            img
                        ));
                    }
                    else
                    {
                        ico = new PackIcon() { Kind = PackIconKind.None };
                        if (Enum.TryParse<PackIconKind>(img, true, out PackIconKind val) is true)
                        {
                            ((PackIcon)ico).Kind = val;
                        }
                        else
                        {
                            img = "";
                        }
                    }
                }
                if (img.Length is 0)
                {
                    ico ??= new PackIcon();
                    ((PackIcon)ico).Kind = type switch
                    {
                        "link" => PackIconKind.ExternalLink,
                        "program" => PackIconKind.Application,
                        "folder" => PackIconKind.Folder,
                        "file" => PackIconKind.File,
                        _ => PackIconKind.None,
                    };
                }
                var lbl = new TextBlock()
                {
                    Text = infoSplit[1]
                };
                lbl.SetBinding(TextBlock.FontSizeProperty, new Binding("DataContext.FontSize_H4"));
                var sp = new StackPanel() { };
                sp.Children.Add(ico);
                sp.Children.Add(lbl);
                var cc = new ContentControl() {
                    Template = ct,
                    Tag = infoSplit[2],
                    Content = sp
                };
                switch (type)
                {
                    case "link":
                        this.SPnl_Shortcuts_Links.Children.Add(cc);
                        break;
                    case "program":
                        this.SPnl_Shortcuts_Programs.Children.Add(cc);
                        break;
                    case "file":
                    case "folder":
                        this.SPnl_Shortcuts_FilesFolders.Children.Add(cc);
                        break;
                    default:
                        break;
                }
            }
        }

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<NotifyChangesEventArgs<FileSystemInfo>>? Home_New_LocalFolders;
        public event EventHandler<NotifyChangesEventArgs<FileSystemInfo>>? Home_New_EngCheck1;
        public event EventHandler<NotifyChangesEventArgs<FileSystemInfo>>? Home_New_EngCheck2;
        public event EventHandler<NotifyChangesEventArgs<FileSystemInfo>>? Home_New_EngReleases;
        public event EventHandler<NotifyChangesEventArgs<FileSystemInfo>>? Home_New_CncWksWorking;
        public event EventHandler<NotifyChangesEventArgs<FileSystemInfo>>? Home_New_CncPnlWorking;
        #endregion

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        readonly System.Timers.Timer Refresher = new(TimeSpan.FromSeconds(10)) { };

        public static Main_ViewModel MainViewModel { get => Ext.MainViewModel; }
        internal static MainWindow ParentWindow { get => Ext.MainWindow; }
        public static double DataGridFontSize { get => Ext.MainViewModel.FontSize_Base; }



        public class PathEntry
        {
            public string Name { get; set; } = "";
            private FileSystemInfo? Path { get; set; }
            public void SetPath(FileSystemInfo fsi)
                => this.Path = fsi;
            public FileSystemInfo? GetPath()
                => this.Path;
        }

        public class RecentOrder
        {
            public string JobNbr { get; set; } = "";
            public string OrderNbr { get; set; } = "";
        }


        private List<RecentOrder> _newOrders = [];
        public IReadOnlyList<RecentOrder> NewOrders => _newOrders;

        public class EngOrder
        {
            public string Name { get; set; } = "";
            public bool PreEng { get; set; } = false;
            public bool Eng { get; set; } = false;
        }
        public List<EngOrder> _engWorking = [];
        public IReadOnlyList<EngOrder> EngWorking => _engWorking;


        public List<EngArchiveEntry> _localFolders  = [];
        public IReadOnlyList<EngArchiveEntry> LocalFolders => _localFolders;


        public List<EngArchiveEntry> _engCheck1 = [];
        public IReadOnlyList<EngArchiveEntry> EngCheck1 => _engCheck1;
        public List<EngArchiveEntry> _engCheck2 = [];
        public IReadOnlyList<EngArchiveEntry> EngCheck2 => _engCheck2;


        public class DirEntryWithParentName: PathEntry
        {
            public string Parent { get; set; } = "";
            public DirEntryWithParentName(string name, DirectoryInfo path)
            {
                this.Name = name;
                this.SetPath(path);
                this.Parent = path.Parent?.Name ?? "";
            }
        }
        public List<DirEntryWithParentName> _engReleases = [];
        public IReadOnlyList<DirEntryWithParentName> EngReleases => _engReleases;

        public class EngArchiveEntry: PathEntry
        {
            public EngArchiveEntry(string name, FileSystemInfo path)
            {
                this.Name = name;
                this.SetPath(path);
            }
        }
        public List<EngArchiveEntry> _engArchive = [];
        public IReadOnlyList<EngArchiveEntry> EngArchive => _engArchive;

        public class CncEntry: PathEntry
        {
            public CncEntry(DirectoryInfo path)
            {
                this.SetPath(path);
            }
        }

        public List<CncEntry> _cncWksEntries = [];
        public IReadOnlyList<CncEntry> CncWksEntries => _cncWksEntries;
        public List<CncEntry> _cncPnlEntries = [];
        public IReadOnlyList<CncEntry> CncPnlEntries => _cncPnlEntries;




        //

        public IQueryable<DBModels.Order.AIcColorSet> GetColorSets(DBModels.Order.OrderDbCtx ctx)
        {
            var now = DateTime.Now;
            var cutoff = DateTime.Now.AddDays(-30);

            // Safe read-only query
            return ctx.AIcColorSets
                .Where(o => o.OrderDate <= cutoff)
                .Where(o => o.SupplyOrderRef.Substring(0, 1) == "S" || o.SupplyOrderRef.Substring(0, 1) == "J")
                .OrderByDescending(o => o.OrderDate)
                .Take(25)
                .AsNoTracking() // No change tracking
                .AsSplitQuery();
        }

        public async Task<List<RecentOrder>> LoadColorSetsAsync()
        {
            using var context = new DBModels.Order.OrderDbCtx();
            var now = DateTime.Now;
            var cutoff = DateTime.Now.AddDays(-30);

            // Safe read-only query
            return await GetColorSets(context)
                .Select(o => new RecentOrder
                {
                    JobNbr = o.SupplyOrderRef ?? "",
                    OrderNbr = o.OrderNumber ?? ""
                })
                .ToListAsync();
        }

        public async Task<List<EngOrder>> GetColorSetEngOrderAsync()
        {
            using var context = new DBModels.Order.OrderDbCtx();
            return await GetColorSets(context)
                .Where(o => o.Engined == false)
                .Select(o => new EngOrder
                {
                    Name = o.SupplyOrderRef ?? "",
                    PreEng = o.Preengined,
                    Eng = o.Engined
                }).ToListAsync();
        }

        public async void GetNewOrders()
        {
            _newOrders = await this.LoadColorSetsAsync();
            OnPropertyChanged(nameof(NewOrders));
        }

        public async void GetEngWorking()
        {
            var now = DateTime.Now;
            var cutoff = DateTime.Now.AddDays(-30);
            _engWorking = await GetColorSetEngOrderAsync();
            OnPropertyChanged(nameof(EngWorking));
        }

        public void GetLocalFolders()
        {
            IList<FileSystemInfo> temp = [];
            IList<FileSystemInfo> added = [];
            IList<FileSystemInfo> removed = [];
            var dirs = new DirectoryInfo("C:/").GetDirectories("*", SearchOption.TopDirectoryOnly);
            foreach (var entry in dirs)
            {
                if (!(entry.Name[..2] is "J0" or "S0"))
                {
                    continue;
                }
                if (!_localFolders.Any(e => e.GetPath()?.FullName == entry.FullName))
                {
                    added.Add(entry);
                    Debug.WriteLine(entry.FullName);
                }
                temp.Add(entry);
            }
            for (short i = 0; i < _localFolders.Count; i++) {
                if (_localFolders[i].GetPath() is not FileSystemInfo fsi) return;
                if (!temp.Any(e => e.FullName == fsi.FullName))
                {
                    removed.Add(fsi);
                }
            }
            _localFolders = [.. temp.Select(e => new EngArchiveEntry(e.Name, e))];
            if (added.Count is not 0 || removed.Count is not 0)
            {
                OnPropertyChanged(nameof(LocalFolders));
                this.Home_New_LocalFolders?.Invoke(this, new("local", added, removed));
            }
        }

        public const string EngCheck1Root = "H:\\engineering\\4. 1st Check";
        public const string EngCheck2Root = "H:\\engineering\\5. 2nd Check";
        public void GetEngChecks()
        {
            var dirs1 = new DirectoryInfo(EngCheck1Root).GetDirectories("*", SearchOption.TopDirectoryOnly);
            var dirs2 = new DirectoryInfo(EngCheck2Root).GetDirectories("*", SearchOption.TopDirectoryOnly);
            List<EngArchiveEntry> temp = [];
            foreach (var entry in dirs1)
            {
                if (!(entry.Name[..1] is "J" or "S"))
                {
                    continue;
                }
                temp.Add(new(entry.Name, entry));
            }
            _engCheck1 = [.. temp];
            temp.Clear();
            OnPropertyChanged(nameof(EngCheck1));
            foreach (var entry in dirs2)
            {
                if (!(entry.Name[..1] is "J" or "S"))
                {
                    continue;
                }
                temp.Add(new(entry.Name, entry));
            }
            _engCheck2 = temp;
            OnPropertyChanged(nameof(EngCheck2));
        }

        public const string EngReleaseDir = "H:\\engineering\\Ready to Release";
        public void GetEngReleases()
        {
            _engReleases.Clear();
            var userDirs = new DirectoryInfo(EngReleaseDir).GetDirectories("*", SearchOption.TopDirectoryOnly);
            var jobDirs = userDirs.SelectMany(d => d.EnumerateDirectories("*", SearchOption.TopDirectoryOnly));
            List<DirEntryWithParentName> temp = [];
            foreach (var entry in jobDirs)
            {
                if (!(entry.Name[..1] is "J" or "S"))
                {
                    continue;
                }
                temp.Add(new(entry.Name, entry));
            }
            _engReleases = temp;
            OnPropertyChanged(nameof(EngReleases));
        }

        public const string EngArchiveRoot = "H:\\Zipped Jobs";
        public DirectoryInfo? EngArchiveCurrentPath;
        public void GetEngArchive()
        {
            this.EngArchiveCurrentPath ??= new(EngArchiveRoot);
            var dirs = EngArchiveCurrentPath.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly);
            List<EngArchiveEntry> temp = [];
            foreach (var entry in dirs)
            {
                if (entry is FileInfo && !entry.Name.EndsWith(".zip"))
                {
                    continue;
                }
                if (entry is DirectoryInfo && !(entry.Name[..1] is "J" or "S"))
                {
                    continue;
                }
                temp.Add(new(entry.Name, entry));
            }
            _engArchive = temp;
            OnPropertyChanged(nameof(EngArchive));
        }

        public const string CncWorkingWksRoot = "H:\\CNC JOBS\\_WORK\\Laid Out";
        public const string CncWorkingPnlRoot = "H:\\CNC JOBS\\_WORK\\PNL Working";
        public void GetCncWorking()
        {
            _cncWksEntries.Clear();
            _cncPnlEntries.Clear();
            var wksEntries = new DirectoryInfo(CncWorkingWksRoot).GetDirectories("*", SearchOption.TopDirectoryOnly);
            var pnlEntries = new DirectoryInfo(CncWorkingPnlRoot).GetDirectories("*", SearchOption.TopDirectoryOnly);
            List<CncEntry> temp = [];
            foreach (var entry in wksEntries)
            {
                if (!(entry.Name[..1] is "J" or "S"))
                {
                    continue;
                }
                temp.Add(new(entry) { Name = entry.Name });
            }
            _cncWksEntries = [.. temp];
            temp.Clear();
            OnPropertyChanged(nameof(CncWksEntries));
            foreach (var entry in pnlEntries)
            {
                if (!(entry.Name[..1] is "J" or "S"))
                {
                    continue;
                }
                temp.Add(new(entry) { Name = entry.Name });
            }
            _cncPnlEntries = temp;
            OnPropertyChanged(nameof(CncPnlEntries));
        }



        //




        public void ChangeEngArchivePath(FileSystemInfo newPath)
            => this.ChangeEngArchivePath(newPath.FullName);

        public void BrowsableListItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton is not MouseButton.Left) return;
            if (sender is not DataGrid dg) return;
            if (dg.SelectedItem is not object item) return;
            if (item.GetType().GetMethod("GetPath", BindingFlags.Public | BindingFlags.Instance) is not MethodInfo meth) return;
            var obj = meth.Invoke(item, []);
            if (obj is null) return;
            if (obj is FileSystemInfo fsi)
            {
                Process.Start("explorer.exe", "/select," + fsi.FullName);
            }
            else if (obj is DirectoryInfo di)
            {
                Process.Start("explorer.exe", "/select," + di.FullName);
            }
        }

        public void ChangeEngArchivePath(string newPath)
        {
            if (!Path.Exists(newPath)) return;
            if (Path.HasExtension(newPath))
            {
                Process.Start("explorer.exe", "/select," + newPath);
                return;
            }
            if (newPath[0] is not 'H' ||
                newPath.Length < EngArchiveRoot.Length)
            {
                newPath = EngArchiveRoot;
            }
            this.EngArchiveCurrentPath = new DirectoryInfo(newPath);
            this.GetEngArchive();
        }

        private void DataGrid_EngArchive_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton is not MouseButton.Left) return;
            if (this.DataGrid_EngArchive.SelectedItem is not EngArchiveEntry entry) return;
            if (entry.GetPath() is not FileSystemInfo fsi || fsi.Exists is false) return;
            this.ChangeEngArchivePath(fsi);
        }

        private void DataGrid_EngArchive_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Enter or Key.Back)
            {
                e.Handled = true;
                if (this.DataGrid_EngArchive.SelectedItem is not EngArchiveEntry entry) return;
                if (entry.GetPath() is not FileSystemInfo fsi) return;
                string? newDir = null;
                switch (e.Key)
                {
                    case Key.Enter:
                        if (fsi.Exists is false) return;
                        newDir = fsi.FullName;
                        break;
                    case Key.Back:
                        if (this.EngArchiveCurrentPath?.Parent is not DirectoryInfo parentDir) return;
                        if (parentDir.Exists is false) return;
                        newDir = parentDir.FullName;
                        break;
                    default:
                        break;
                }
                if (newDir is null) return;
                this.ChangeEngArchivePath(newDir);
            }
        }

        private void Ctx_Job_OpenInApp_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null) return;
            if (sender is MenuItem mi &&
                mi.Parent is ContextMenu cm &&
                cm.PlacementTarget is object obj)
            {
                if (obj is not DataGridRow dgr || dgr.DataContext is not PathEntry pe) return;
                MainViewModel.CurrentPage = PageTypes.EngOrder;
                MainViewModel.EngOrder_VM?.JobNbr = pe.Name;
            }
            if (sender is Button btn)
            {
                if (btn.DataContext is not PathEntry pe) return;
                MainViewModel.CurrentPage = PageTypes.EngOrder;
                MainViewModel.EngOrder_VM?.JobNbr = pe.Name;
            }
        }

        private void Ctx_Job_OpenFolderInExplorer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null) return;
            if (sender is MenuItem mi &&
                mi.Parent is ContextMenu cm &&
                cm.PlacementTarget is object obj)
            {
                if (obj is not DataGridRow dgr || dgr.DataContext is not PathEntry pe || pe.GetPath() is not FileSystemInfo fsi) return;
                Process.Start("explorer.exe", fsi.FullName);
            }
            if (sender is Button btn)
            {
                if (btn.DataContext is not PathEntry pe || pe.GetPath() is not FileSystemInfo fsi) return;
                Process.Start("explorer.exe", fsi.FullName);
            }
        }

        private void Ctx_Job_OpenContainingFolderInExplorer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null) return;
            if (sender is MenuItem mi &&
                mi.Parent is ContextMenu cm &&
                cm.PlacementTarget is object obj)
            {
                if (obj is not DataGridRow dgr || dgr.DataContext is not PathEntry pe || pe.GetPath() is not FileSystemInfo fsi) return;
                Process.Start("explorer.exe", "/select," + fsi.FullName);
            }
            if (sender is Button btn)
            {
                if (btn.DataContext is not PathEntry pe || pe.GetPath() is not FileSystemInfo fsi) return;
                Process.Start("explorer.exe", "/select," + fsi.FullName);
            }
        }

        private void DataGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is null) return;
            if (sender is not DataGrid dg) return;
            dg.SelectedIndex = -1;
        }

        private void Refresher_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            this.LoadData();
        }

        private void btn_reload_Click(object sender, RoutedEventArgs e)
        {
            this.LoadData();
            Ext.MainWindow.MainToastContainer.CreateToast("Home", "Data refreshed").Show();
        }

        private void Btn_HomeNotifs_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag?.ToString()?.ToLower() is not string tag) return;
            Debug.WriteLine(tag);
            ToggleNotifs(tag);
        }

        public class NotifyChangesEventArgs<T>(string sourceName, IList<T> added, IList<T> removed)
        {
            public string SourceName { get; init; } = sourceName;
            public IList<T> Added { get; init; } = added;
            public IList<T> Removed { get; init; } = removed;
        }


        private void Notification_Handler(object? sender, object e)
        {
            if (e is not NotifyChangesEventArgs<object>) return;
            if (e is NotifyChangesEventArgs<FileSystemInfo> eargs)
            {
                switch (eargs.SourceName)
                {
                    case "local":
                        Ext.MainWindow.MainToastContainer.CreateToast(
                            "Home Notify",
                            $"New 'local' entries: {string.Join(", ", eargs.Added)}"
                        ).Show();
                        break;
                    default:
                        break;
                }
            }
        }

        public void ToggleNotifs(string type)
        {
            var settingName = $"Notify_{type}";
            if (Ext.ReadSettingAsBool(settingName) is (bool, bool) res && res.success is false) return;
            Debug.WriteLine($"value read: {res.value}");
            bool currentVal = res.value,
                 newVal = !res.value;
            Ext.AddUpdateAppSettings(settingName, $"{newVal}");
            switch (type)
            {
                case "local":
                    if (newVal)
                    {
                        this.Home_New_LocalFolders += this.Notification_Handler;
                        Debug.WriteLine("Handler added");
                    }
                    else
                    {
                        this.Home_New_LocalFolders -= this.Notification_Handler;
                        Debug.WriteLine("Handler removed");
                    }
                    break;
                default:
                    break;
            }
            Ext.MainWindow.MainToastContainer.CreateToast("Home", $"Notifications {(newVal ? "enabled" : "disabled")} for '{type}'").Show();
        }

        private void Btn_Shortcuts_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not string tag) return;
            var psi = new ProcessStartInfo(tag)
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(psi);
        }


        public const byte HomeSectionCollapsedHeight = 12;
        public const byte HomeSectionShortcutsRegularHeight = 120;
        public const byte HomeSectionRegularHeight = 200;
        private void Btn_ToggleSectionVis(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag?.ToString()?.ToLower() is not string tag) return;
            FrameworkElement? c = tag switch
            {
                "quick access" => this.SPnl_Shortcuts,
                "orders" => this.SPnl_Section_Orders,
                "engineering" => this.SPnl_Section_Engineering,
                "cnc" => this.SPnl_Section_CNC,
                _ => null
            };
            if (c is null) return;
            bool isHidden = c.Height is HomeSectionCollapsedHeight;
            c.Height = (isHidden ? (tag is "quick access" ? HomeSectionShortcutsRegularHeight : HomeSectionRegularHeight) : HomeSectionCollapsedHeight);
            c.Visibility = (isHidden ? Visibility.Visible : Visibility.Hidden);
            var icon = (btn.Content as FrameworkElement)?.FindName("PackIco_SectionVisState") as PackIcon;
            if (icon is null) return;
            icon.Kind = (isHidden ? PackIconKind.VisibilityOutline : PackIconKind.VisibilityOffOutline);
        }
    }
}
