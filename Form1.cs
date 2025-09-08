namespace OMPS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            this.webView21.CoreWebView2InitializationCompleted += WebView21_CoreWebView2InitializationCompleted;
            await this.webView21.EnsureCoreWebView2Async();
        }

        private void WebView21_CoreWebView2InitializationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            this.webView21.CoreWebView2.NavigateToString("/index.html");
        }
    }
}
