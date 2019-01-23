using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xerox_operations.api;

namespace xerox_operations_0._0._1
{

    class DirectoryMonitor
    {
        private MainForm myForm;

        private FileSystemWatcher fsw;
        public StringBuilder log;       
        private string directoryPath;

        public DirectoryMonitor(MainForm f, string directoryPath)
        {
            this.myForm = f;
            this.directoryPath = directoryPath;

            if (Directory.Exists(directoryPath))
            {
                fsw = new FileSystemWatcher(Path.Combine(Path.GetDirectoryName(directoryPath)));
                fsw.IncludeSubdirectories = true;
                initFIlters();
            }
            log = new StringBuilder();
        }

        private void initFIlters()
        {
            fsw.NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.FileName;
            fsw.Changed += new FileSystemEventHandler(OnChanged);
            fsw.Created += new FileSystemEventHandler(OnChanged);
            // fsw.Deleted += new FileSystemEventHandler(OnDeleted);
            fsw.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            string mgs = string.Format(e.ChangeType + " - " + string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + " - " + "{0}", e.Name);
                    
            if (e.ChangeType.ToString().Equals("Created"))
            {
                log.Append(mgs + "\r\n");
                new PushNotifications(string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now), e.Name);
            }

            switch (directoryPath)
            {
                case Paths.B2B_PATH:
                    myForm.richTextBoxB2B_TextChanged(sender, e);
                    break;
                case Paths.B2C_PATH:
                    myForm.richTextBoxB2C_TextChanged(sender, e);
                    break;
            }
            Console.WriteLine(mgs);
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            string mgs = string.Format("Deleted - " + string.Format("{0:yyyy-MM-dd hh:mm:ss}", DateTime.Now) + " - " + "{0}", e.Name);
            log.Append(mgs + "\r\n");

            switch (directoryPath)
            {
                case Paths.B2B_PATH:
                    myForm.richTextBoxB2B_TextChanged(sender, e);
                    break;
                case Paths.B2C_PATH:
                    myForm.richTextBoxB2C_TextChanged(sender, e);
                    break;
            }
            Console.WriteLine(mgs);
        }

        public void clearLog()
        {
            log.Clear();
        }

        public String getFileName()
        {
            if (log != null)
            {
                return log.ToString();
            }
            else return "Some error occur";
        }

    }
}
