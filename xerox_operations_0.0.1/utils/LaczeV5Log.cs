using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using xerox_operations_0._0._1;

namespace xerox_operations.utils
{


    public class LaczeV5Log
    {
        private MainForm myForm;
        private FileSystemWatcher fsw;
        public StringBuilder log;
        private string directoryPath = @"D:\Jonit_VAR\Prod\log\";
        private string filePath = @"D:\Jonit_VAR\Prod\log\dupa.txt";

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public LaczeV5Log(MainForm f)
        {
            this.myForm = f;            

            if (Directory.Exists(directoryPath))
            {
                fsw = new FileSystemWatcher(Path.Combine(Path.GetDirectoryName(filePath)));
                fsw.IncludeSubdirectories = true;                
                initFIlters();
            }
            log = new StringBuilder();
        }

        private void initFIlters()
        {
            fsw.NotifyFilter = NotifyFilters.LastWrite;
            fsw.Changed += new FileSystemEventHandler(OnChanged);           
            fsw.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            log.Clear();
            try
            {
                using (StreamReader r = new StreamReader(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    foreach (var line in File.ReadAllLines(filePath))
                    {
                        log.Append(line.ToString() + "\r\n");                        
                    }
                }
            }
            catch (Exception ex) { }
            myForm.richTextBox1_TextChanged(sender,e);
        }

        private string readLine()
        {                        
            try
            {
                string s;

                using (StreamReader r = new StreamReader(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    foreach (var line in File.ReadAllLines(filePath))
                    {
                        log.Append(line.ToString() + "\r\n");
                    }
                }
            } 
            catch (Exception e){ }
            return "No data";
        }

        public string getLog()
        {
            if (log != null)
            {
                return log.ToString();
            }
            else return "Some error occur";            
        }
    }
}
