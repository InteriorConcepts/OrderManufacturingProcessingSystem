using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Windows.Forms.Design;

namespace OMPS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //
            WebView_Init(this.webView21);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {

        }

        public FileInfo ExeFileInfo = new (Application.ExecutablePath);
        public string? CurrentWorkingDirectory()
        {
            return ExeFileInfo?.Directory?.FullName;
        }

        private async Task<CoreWebView2Environment> WebView_InitEnvironment()
        {
            var name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            var tempFolder = Path.Combine(Path.GetTempPath(), name, DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
            return await CoreWebView2Environment.CreateAsync(null, tempFolder, new CoreWebView2EnvironmentOptions());
        }

        private async void WebView_Init(WebView2 webView)
        {
            webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
            await webView.EnsureCoreWebView2Async(await WebView_InitEnvironment());
            webView.CoreWebView2.AddHostObjectToScript("BackendApi", new HostObjectApi());
        }

        private void WebView_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                MessageBox.Show($"Was unable to Initialize WebView control!\n{e.InitializationException.Message}");
                return;
            }
            else
            {
                this.webView21.CoreWebView2.Settings.IsZoomControlEnabled = false;
            }

            var cwd = this.CurrentWorkingDirectory();
            if (cwd is null)
            {
                MessageBox.Show("Couldn't get exe current directory or returned null. Exiting..");
                return;
            }
            this.webView21.CoreWebView2.SetVirtualHostNameToFolderMapping("wwwroot", $"{cwd}/wwwroot/", CoreWebView2HostResourceAccessKind.Allow);
            this.webView21.CoreWebView2.Navigate($"{cwd}/wwwroot/html/index.html");
        }
    }
}
