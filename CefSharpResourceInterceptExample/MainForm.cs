using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.Callback;
using CefSharp.Handler;
using CefSharp.WinForms;

namespace CefSharpResourceInterceptExample
{
    public partial class MainForm : Form
    {
        private readonly ChromiumWebBrowser _webBrowser;
        public MainForm()
        {
            InitializeComponent();
            this._webBrowser = new ChromiumWebBrowser(string.Empty)
            {
                RequestHandler = new MyRequestHandler()
            };
            this.PnlMain.Controls.Add(this._webBrowser);
        }

        private void BtnGo_Click(object sender, EventArgs e)
        {
            this._webBrowser.Load(this.TxtBoxUrl.Text.Trim());
        }
    }

    public class MyRequestHandler : RequestHandler
    {
        protected override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture,
            bool isRedirect)
        {
            // 先调用基类的实现，断点调试
            return base.OnBeforeBrowse(chromiumWebBrowser, browser, frame, request, userGesture, isRedirect);
        }

        protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            // 先调用基类的实现，断点调试
            return new MyResourceRequestHandler();
        }
    }

    public class MyResourceRequestHandler : ResourceRequestHandler
    {
        protected override IResourceHandler GetResourceHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            return new MyResourceHandler();
        }
    }

    public class MyResourceHandler : IResourceHandler
    {
        public void Dispose()
        {
            
        }

        public bool Open(IRequest request, out bool handleRequest, ICallback callback)
        {
            handleRequest = true;
            return true;
        }

        public bool ProcessRequest(IRequest request, ICallback callback)
        {
            throw new NotImplementedException();
        }

        public void GetResponseHeaders(IResponse response, out long responseLength, out string redirectUrl)
        {
            throw new NotImplementedException();
        }

        public bool Skip(long bytesToSkip, out long bytesSkipped, IResourceSkipCallback callback)
        {
            throw new NotImplementedException();
        }

        public bool Read(Stream dataOut, out int bytesRead, IResourceReadCallback callback)
        {
            throw new NotImplementedException();
        }

        public bool ReadResponse(Stream dataOut, out int bytesRead, ICallback callback)
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }
    }
}
