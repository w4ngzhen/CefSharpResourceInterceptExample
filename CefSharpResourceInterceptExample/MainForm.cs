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
            if (request.Url.EndsWith("test1.js") || request.Url.EndsWith("test1.css"))
            {
                MessageBox.Show($@"资源拦截：{request.Url}");

                string type = request.Url.EndsWith(".js") ? "js" : "css"; // 这里简单判断js还是css，不过多编写
                string fileName = null;
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = $@"{type}文件|*.{type}"; // 过滤
                    openFileDialog.Multiselect = true;
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        fileName = openFileDialog.FileName;
                    }
                }

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    // 没有选择文件，还是走默认的Handler
                    return base.GetResourceHandler(chromiumWebBrowser, browser, frame, request);
                }
                // 否则使用选择的资源返回
                return new MyResourceHandler(fileName);
            }

            return base.GetResourceHandler(chromiumWebBrowser, browser, frame, request);
        }
    }

    public class MyResourceHandler : IResourceHandler
    {
        private readonly string _localResourceFileName;
        private byte[] _localResourceData;
        private int _dataReadCount;

        public MyResourceHandler(string localResourceFileName)
        {
            this._localResourceFileName = localResourceFileName;
            this._dataReadCount = 0;
        }

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
            using (FileStream fileStream = new FileStream(this._localResourceFileName, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    long length = fileStream.Length;
                    this._localResourceData = new byte[length];
                    // 读取文件中的内容并保存到私有变量字节数组中
                    binaryReader.Read(this._localResourceData, 0, this._localResourceData.Length);
                }
            }

            responseLength = this._localResourceData.Length;
            redirectUrl = null;
        }

        public bool Skip(long bytesToSkip, out long bytesSkipped, IResourceSkipCallback callback)
        {
            throw new NotImplementedException();
        }

        public bool Read(Stream dataOut, out int bytesRead, IResourceReadCallback callback)
        {
            int leftToRead = this._localResourceData.Length - this._dataReadCount;
            if (leftToRead == 0)
            {
                bytesRead = 0;
                return false;
            }

            int needRead = Math.Min((int)dataOut.Length, leftToRead);
            dataOut.Write(this._localResourceData, this._dataReadCount, needRead);
            this._dataReadCount += needRead;
            bytesRead = needRead;
            return true;
        }

        public bool ReadResponse(Stream dataOut, out int bytesRead, ICallback callback)
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            
        }
    }
}
