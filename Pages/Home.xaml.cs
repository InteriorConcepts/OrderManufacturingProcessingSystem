using Humanizer;
using OMPS.viewModel;
using OMPS.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using Windows.Devices.Geolocation;

namespace OMPS.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl, INotifyPropertyChanged
    {
        public Home() { }
        public Home(MainWindow parentWindow)
        {
            this.ParentWindow = parentWindow;
            InitializeComponent();
            //
            this.DataContext = this;
            this.GetNewOrders();
            this.GetEngWorking();
            this.GetEngChecks();
            this.GetEngReleases();
            this.GetEngArchive();
            this.GetCncWorking();
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Main_ViewModel MainViewModel
        {
            get => Ext.MainWindow.MainViewModel;
        }

        public double DataGridFontSize
        {
            get => MainViewModel.FontSize_Base;
        }

        internal MainWindow ParentWindow
        {
            get; set
            {
                field = value;
                value?.MainViewModel?.PropertyChanged += new((sender, e) =>
                {
                    if (e.PropertyName is not nameof(ParentWindow.MainViewModel.FontSize_Base)) return;
                    //this.datagrid_orders.UpdateLayout();
                    OnPropertyChanged(nameof(DataGridFontSize));
                });
            }
        }

        public class PathEntry
        {
            public string Name { get; set; } = "";
            private FileSystemInfo Path { get; set; }
            public void SetPath(FileSystemInfo fsi)
                => this.Path = fsi;
            public FileSystemInfo GetPath()
                => this.Path;
        }

        public class RecentOrder
        {
            public string JobNbr { get; set; } = "";
            public string QuoteNbr { get; set; } = "";
        }
        public ObservableCollection<RecentOrder> NewOrders { get; set; } = [];

        public class EngOrder
        {
            public string JobNbr { get; set; } = "";
            public bool PreEng { get; set; } = false;
            public bool Eng { get; set; } = false;
        }
        public ObservableCollection<EngOrder> EngWorking { get; set; } = [];


        public ObservableCollection<EngArchiveEntry> EngCheck1 { get; set; } = [];
        public ObservableCollection<EngArchiveEntry> EngCheck2 { get; set; } = [];

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
        public ObservableCollection<DirEntryWithParentName> EngReleases { get; set; } = [];

        public class EngArchiveEntry: PathEntry
        {
            public EngArchiveEntry(string name, FileSystemInfo path)
            {
                this.Name = name;
                this.SetPath(path);
            }
        }
        public ObservableCollection<EngArchiveEntry> EngArchive { get; set; } = [];

        public class CncEntry: PathEntry
        {
            public CncEntry(DirectoryInfo path)
            {
                this.SetPath(path);
            }
        }
        public ObservableCollection<CncEntry> CncWksEntries { get; set; } = [];
        public ObservableCollection<CncEntry> CncPnlEntries { get; set; } = [];




        //




        public void GetNewOrders()
        {
            NewOrders.Clear();
            var now = DateTime.Now;
            var data_orders =
                Ext.Queries.GetColorSets("%").
                    Where(i => (now - i.OrderDate).TotalDays <= 30).
                    OrderBy(i => now - i.OrderDate).
                    Take(20);
            foreach (var item in data_orders)
            {
                NewOrders.Add(new () { JobNbr = item.JobNbr, QuoteNbr = item.OrderNumber});
            }
            //OnPropertyChanged(nameof(NewOrders));
        }

        public void GetEngWorking()
        {
            EngWorking.Clear();
            var now = DateTime.Now;
            var jobs = Ext.Queries.GetColorSets("%").Select(i => i.JobNbr);
            var colorSetDatas =
                jobs.SelectMany(Ext.Queries.GetColorSet).
                Where(i => i.Engined is false);
            foreach (var item in colorSetDatas)
            {
                EngWorking.Add(new EngOrder { JobNbr = item.SupplyOrderRef, PreEng = item.Preengined, Eng = item.Engined });
            }
            //OnPropertyChanged(nameof(EngWorking));
        }

        public const string EngCheck1Root = "H:\\engineering\\4. 1st Check";
        public const string EngCheck2Root = "H:\\engineering\\5. 2nd Check";
        public void GetEngChecks()
        {
            EngCheck1.Clear();
            EngCheck2.Clear();

            var dirs1 = new DirectoryInfo(EngCheck1Root).GetDirectories("*", SearchOption.TopDirectoryOnly);
            var dirs2 = new DirectoryInfo(EngCheck1Root).GetDirectories("*", SearchOption.TopDirectoryOnly);
            foreach (var entry in dirs1)
            {
                if (!(entry.Name[..1] is "J" or "S"))
                {
                    continue;
                }
                EngCheck1.Add(new(entry.Name, entry));
            }
            foreach (var entry in dirs2)
            {
                if (!(entry.Name[..1] is "J" or "S"))
                {
                    continue;
                }
                EngCheck2.Add(new(entry.Name, entry));
            }
        }

        public const string EngReleaseDir = "H:\\engineering\\Ready to Release";
        public void GetEngReleases()
        {
            var userDirs = new DirectoryInfo(EngReleaseDir).GetDirectories("*", SearchOption.TopDirectoryOnly);
            var jobDirs = userDirs.SelectMany(d => d.EnumerateDirectories("*", SearchOption.TopDirectoryOnly));

            foreach (var entry in jobDirs)
            {
                if (!(entry.Name[..1] is "J" or "S"))
                {
                    continue;
                }
                EngReleases.Add(new(entry.Name, entry));
            }
        }

        public const string EngArchiveRoot = "H:\\Zipped Jobs";
        public DirectoryInfo? EngArchiveCurrentPath;
        public void GetEngArchive()
        {
            this.EngArchiveCurrentPath ??= new(EngArchiveRoot);
            EngArchive.Clear();
            var dirs = EngArchiveCurrentPath.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly);
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
                EngArchive.Add(new(entry.Name, entry));
            }
        }

        public const string CncWorkingWksRoot = "H:\\CNC JOBS\\_WORK\\Laid Out";
        public const string CncWorkingPnlRoot = "H:\\CNC JOBS\\_WORK\\PNL Working";
        public void GetCncWorking()
        {
            CncWksEntries.Clear();
            CncPnlEntries.Clear();
            var wksEntries = new DirectoryInfo(CncWorkingWksRoot).GetDirectories("*", SearchOption.TopDirectoryOnly);
            var pnlEntries = new DirectoryInfo(CncWorkingPnlRoot).GetDirectories("*", SearchOption.TopDirectoryOnly);
            foreach (var entry in wksEntries)
            {
                if (!(entry.Name[..1] is "J" or "S"))
                {
                    continue;
                }
                CncWksEntries.Add(new(entry) { Name = entry.Name });
            }
            foreach (var entry in pnlEntries)
            {
                if (!(entry.Name[..1] is "J" or "S"))
                {
                    continue;
                }
                CncPnlEntries.Add(new(entry) { Name = entry.Name });
            }
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
            if (entry.GetPath().Exists is false) return;
            this.ChangeEngArchivePath(entry.GetPath());
        }

        private void DataGrid_EngArchive_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Enter or Key.Back)
            {
                e.Handled = true;
                if (this.DataGrid_EngArchive.SelectedItem is not EngArchiveEntry entry) return;
                string? newDir = null;
                switch (e.Key)
                {
                    case Key.Enter:
                        if (entry.GetPath().Exists is false) return;
                        newDir = entry.GetPath().FullName;
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
            if (sender is not MenuItem mi ||
                mi.Parent is not ContextMenu cm ||
                cm.PlacementTarget is not object obj ||
                obj is not DataGridRow dgr) return;
            if (dgr.DataContext is not PathEntry pe) return;
            this.MainViewModel.CurrentPage = PageTypes.EngOrder;
            this.MainViewModel.EngOrder_VM.JobNbr = pe.Name;
        }

        private void Ctx_Job_OpenFolderInExplorer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null) return;
            if (sender is not MenuItem mi ||
                mi.Parent is not ContextMenu cm ||
                cm.PlacementTarget is not object obj ||
                obj is not DataGridRow dgr) return;
            if (dgr.DataContext is not PathEntry pe) return;
            Process.Start("explorer.exe", pe.GetPath().FullName);
        }

        private void Ctx_Job_OpenContainingFolderInExplorer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is null) return;
            if (sender is not MenuItem mi ||
                mi.Parent is not ContextMenu cm ||
                cm.PlacementTarget is not object obj ||
                obj is not DataGridRow dgr) return;
            if (dgr.DataContext is not PathEntry pe) return;
            Process.Start("explorer.exe", "/select," + pe.GetPath().FullName);
        }
    }
}
