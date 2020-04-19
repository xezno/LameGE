using ECSEngine.DebugUtils;
using ECSEngine.Events;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace ECSEngine.Managers
{
    class RconWebFrontendManager : Manager<RconWebFrontendManager>
    {
        private HttpListener listener;
        private Thread httpThread;
        private bool shouldRun = true;

        public RconWebFrontendManager()
        {
            if (!GameSettings.Default.rconEnabled)
                return;

            listener = new HttpListener();
            listener.Prefixes.Add($"http://127.0.0.1:{GameSettings.Default.webPort}/");
            listener.Start();

            Logging.Log($"Web console is listening on :{GameSettings.Default.webPort}");

            httpThread = new Thread(HttpThread);
            httpThread.Start();
        }

        private void HttpThread()
        {
            while (shouldRun)
            {
                var httpListenerContext = listener.GetContext();
                var request = httpListenerContext.Request;
                var response = httpListenerContext.Response;

                // Check which file was requested; if none, use index.html
                var fileRequested = request.Url.AbsolutePath;
                if (fileRequested == "/")
                    fileRequested = "/index.html";

                var filePath = $"Content/WebConsole/{fileRequested}";

                if (File.Exists(filePath))
                {
                    byte[] data = File.ReadAllBytes(filePath);

                    response.ContentType = GetContentType(filePath);
                    response.ContentEncoding = Encoding.Unicode;
                    response.ContentLength64 = data.Length;

                    response.OutputStream.Write(data, 0, data.Length);
                    response.Close();
                }
                else
                {
                    response.ContentType = "none";
                    response.StatusCode = 404;
                    response.Close();
                }
            }
        }

        private string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath)?.Substring(1);
            switch (extension)
            {
                case "css":
                    return "text/css";
                case "html":
                    return "text/html";
                case "js":
                    return "text/javascript";
                default:
                    return "text/plain";
            }
        }

        public override void HandleEvent(Event eventType, IEventArgs eventArgs)
        {
            if (eventType != Event.GameEnd)
                return;

            shouldRun = false;
            listener.Close();
        }
    }
}
