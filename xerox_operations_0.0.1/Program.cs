using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Windows.Forms;
using xerox_operations_0._0._1.api;

namespace xerox_operations_0._0._1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool isAdmin;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            
            if (isAdmin)
            {
                onCreateApiServer();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }

        public static void onCreateWebRequest(string localHttp)
        {
            try
            {
                WebRequest request = WebRequest.Create(localHttp);
                request.Method = "GET";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            }
            catch (Exception ex)
            {
                string message = "Someone else is using this program, please open it NOT as administrator it will run without HttpSelfHostServer.";
                MessageBox.Show(message + "\r\n\r\n" + ex.StackTrace, ex.Message);
            }
        }

        private static void onCreateApiServer()
        {
            string[] args = Environment.GetCommandLineArgs();
            Process process = Process.GetCurrentProcess();
            // Check how many total processes have the same name as current one
            if (Process.GetProcessesByName(process.ProcessName).Length > 1)
            {
                onCreateWebRequest("http://localhost:19002/Communication/AddItem/");
                onCreateWebRequest("http://localhost:19002/Communication/DeleteItem/");
                onCreateWebRequest("http://localhost:19002/Communication/AddDriveProgress/");
                onCreateWebRequest("http://localhost:19002/Communication/AddDriveFreeSpaceAvailable/");                
            }
            else
            {
                var config = new HttpSelfHostConfiguration("http://localhost:19002");
                config.Routes.MapHttpRoute(
                    name: "API",
                    routeTemplate: "{controller}/{action}/{Item}",
                    defaults: new { item = RouteParameter.Optional }
                );

                using (HttpSelfHostServer server = new HttpSelfHostServer(config))
                {
                    server.OpenAsync().Wait();
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    MainForm form = new MainForm();
                    CommonObjects.mainFormReference = form;
                    Application.Run(form);
                }
            }
        }
    }
}
