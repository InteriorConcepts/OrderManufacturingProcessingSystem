using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Web;

namespace OMPS
{
    public class WebView_EventsHandler
    {
        internal async void CoreWebView2_WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            var requestUri = new Uri(e.Request.Uri);
            string urlStr = e.Request.Uri;
            CoreWebView2WebResourceContext context = e.ResourceContext;
            if (context.ToString() is not "XmlHttpRequest") return;
            if (!requestUri.AbsolutePath.Contains("/api/")) return;
            var apiUrlPart = requestUri.AbsolutePath.Split("/api/")[1];
            var deferral = e.GetDeferral();
            try
            {
                var apiPath = apiUrlPart;
                var parsedUrl = HttpUtility.ParseQueryString(requestUri.Query);
                //Debug.WriteLine(string.Join("\n", [requestUri, context, apiPath, string.Join("\n", parsedUrl.Keys.OfType<string>().Select(k => $"  {k}: {parsedUrl[k]}"))]));
                switch (apiPath)
                {
                    case "getItemLinesByJob":
                        var json = GlobalObjects.GeneratedQueries.GetItemLinesByJob_Web("J000035601");
                        //Debug.WriteLine(json);
                        var resData = StringCompression.GZip.CompressString(
                                GlobalObjects.GeneratedQueries.GetItemLinesByJob_Web("J000035601")
                            );
                        //Debug.WriteLine(resData);
                        //var resStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(resData));
                        e.Response = GlobalObjects.MainForm.webView21.CoreWebView2.Environment.CreateWebResourceResponse(
                            resData,
                            200,
                            "OK",
                            "Content-Type: application/json\r\nContent-Encoding: gzip"
                        );
                        break;
                    default:
                        break;
                }
            }
            finally
            {
                deferral.Complete();
            }

        }

        internal void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            Debug.WriteLine(e.WebMessageAsJson);
        }
    }
}
