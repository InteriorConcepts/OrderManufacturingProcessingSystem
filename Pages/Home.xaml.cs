using OMPS.viewModel;
using OMPS.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Humanizer;
using System.IO;

namespace OMPS.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl, INotifyPropertyChanged
    {
        public Home(MainWindow parentWindow)
        {
            InitializeComponent();
            //
            this.DataContext = this;
            this.ParentWindow = parentWindow;
            this.GetNewOrders();
            this.GetEngWorking();
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
            get => this.ParentWindow.MainViewModel;
        }

        public double DataGridFontSize
        {
            get => this.MainViewModel.FontSize_DataGrid;
        }

        internal MainWindow ParentWindow
        {
            get; set
            {
                field = value;
                value?.MainViewModel?.PropertyChanged += new((sender, e) =>
                {
                    if (e.PropertyName is not nameof(ParentWindow.MainViewModel.FontSize_DataGrid)) return;
                    //this.datagrid_orders.UpdateLayout();
                    OnPropertyChanged(nameof(DataGridFontSize));
                });
            }
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

        public class EngArchiveEntry
        {
            public string Name { get; set; } = "";
            private FileSystemInfo Path { get; set; }
            public FileSystemInfo GetPath()
            {
                return Path;
            }
            public EngArchiveEntry(string name, FileSystemInfo path)
            {
                this.Name = name;
                this.Path = path;
            }
        }
        public ObservableCollection<EngArchiveEntry> EngArchive { get; set; } = [];

        public class CncEntry(DirectoryInfo Path)
        {
            public string Type { get; set; } = "";
            public string Name { get; set; } = "";
            private DirectoryInfo? Path { get; set; }
            public DirectoryInfo? GetPath()
                => this.Path;
        }
        public ObservableCollection<CncEntry> CncEntries { get; set; } = [];




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
            CncEntries.Clear();
            foreach (var entry in new DirectoryInfo(CncWorkingWksRoot).GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                if (!(entry.Name[..1] is "J" or "S"))
                {
                    continue;
                }
                CncEntries.Add(new(entry) { Name = entry.Name, Type = "Wks" });
            }
            foreach (var entry in new DirectoryInfo(CncWorkingPnlRoot).GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                if (!(entry.Name[..1] is "J" or "S"))
                {
                    continue;
                }
                CncEntries.Add(new(entry) { Name = entry.Name, Type = "Pnl" });
            }
        }



        //




        public void ChangeEngArchivePath(FileSystemInfo newPath)
            => this.ChangeEngArchivePath(newPath.FullName);

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
    }
}
