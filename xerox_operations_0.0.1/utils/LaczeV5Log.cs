using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
        private int partNo = 0;
        private const string SEPARATOR = " - ";
        private const string NEXT_LINE = "\r\n";

        private static int EXECUTION_NUMBER;

        private static string LAST_START_TIME = "0";

        private MyLog myLog = new MyLog();

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public LaczeV5Log(MainForm f)
        {
            this.myForm = f;            

            if (Directory.Exists(directoryPath))
            {
                fsw = new FileSystemWatcher(Path.Combine(Path.GetDirectoryName(filePath)));
                fsw.IncludeSubdirectories = false;                
                initFIlters();
            }
            log = new StringBuilder();
        }

        private void initFIlters()
        {
            fsw.NotifyFilter = NotifyFilters.LastWrite;
            fsw.Filter = "dupa.txt";
            fsw.Changed += new FileSystemEventHandler(OnChanged);
            fsw.EnableRaisingEvents = true;
        }

        // Define the event handlers.
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            EXECUTION_NUMBER++;
            using (StreamWriter outputFile = new StreamWriter(Path.Combine("EXECUTION_NUMBER.txt")))
            {
                outputFile.WriteLine(EXECUTION_NUMBER);
            }

            // Preparing object to fetch new informations
            myLog = new MyLog();

            try
            {                
                using (StreamReader r = new StreamReader(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    foreach (var line in File.ReadAllLines(filePath))
                    {
                        //Console.WriteLine("startTime: "+startTime(line));
                        if (startTime(line) != null) myLog.setStartTime(startTime(line));                              
                        if (completeRunTime(line) != null) myLog.setCompleteRunTime(completeRunTime(line));                        
                        if (country(line) != null) myLog.setCountry(country(line));
                        if (sheetsNumber(line) > 0) myLog.setSheetsNumber(sheetsNumber(line));
                        if (error(line)) myLog.setIsError(error(line));
                        if (line.Contains("error") && !line.Contains("Fatal error 0030:Nothing to print.")) myLog.setErrorMessage(errorMessage(line));                        
                    }
                }
                if (!myLog.getStartTime().Equals(LAST_START_TIME)) executeOnlyOnce();
            }
            catch (Exception ex) { }            
            
            // Invoke Method            
            myForm.richTextBox1_TextChanged(sender,e);            
        }

        // It must be done this way because Method "OnChanged" is double Invoked after file modification.
        private void executeOnlyOnce()
        {
            if (!myLog.getIsError())
            {
                // do stuff
                log.Append(myLog.getCountry());                
                log.Append(SEPARATOR);

                log.Append("czas rozpoczęcia: " + myLog.getStartTime());
                LAST_START_TIME = myLog.getStartTime();
                log.Append(SEPARATOR);                

                log.Append("ilość stron: " + myLog.getSheetsNumber());
                log.Append(SEPARATOR);

                log.Append("czas procesowania: " + myLog.getCompleteRunTime());
                log.Append(SEPARATOR);
                log.Append(NEXT_LINE);

                using (StreamWriter w = File.AppendText("log.txt"))
                {
                    Log(myLog.getCountry() + " Czas rozpoczęcia: " + myLog.getStartTime() + " Ilość stron: " + myLog.getSheetsNumber().ToString() + " Czas procesowania: " + myLog.getCompleteRunTime(), w);
                }
            }
            else
            {
                log.Append("### WYSTĄPIŁ ERROR ###");
                log.Append(NEXT_LINE);
                log.Append(myLog.getErrorMessage());
                log.Append(NEXT_LINE);
                myLog = new MyLog();
            }            
        }

        public void Log(string logMessage, TextWriter w)
        {
            w.Write("{0} {1}" , DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString() + "\r\n");
            w.Write("{0}", logMessage);
            w.WriteLine();
        }

        // Catching time
        private string startTime(string s)
        {
            if (s.Contains("Opening workflow"))
            {
                string[] words = s.Split(' ');
                Console.WriteLine(words.Length-5);                
                string processDuration = words[words.Length-1].Replace(".", "");                
                foreach (var asd in words) Console.WriteLine(asd.ToString());

                Console.WriteLine("startTime: " + trimTime(processDuration, 8));
                return trimTime(processDuration, 8);
            }
            return null;
        }

        // Catching runtime
        private string completeRunTime(string s)
        {
            if (s.Contains("Complete run time"))
            {
                string[] words = s.Split(' ');

                string processDuration = words[5].Replace(".", "");

                Console.WriteLine("completeRunTime: " + trimTime(processDuration, 7));
                return trimTime(processDuration, 7);
            }
            return null;
        }

        // Catching proper country
        private string country(string s)
        {
            if (s.Contains("PL")) return "PL";
            if (s.Contains("CZ")) return "CZ";
            if (s.Contains("SIN")) return "SI";
            if (s.Contains("HU")) return "HU";
            if (s.Contains("SK")) return "SK";
            else return null;
        }

        // Catching sheets number
        private int sheetsNumber(string s)
        {
            if (s.Contains("Status message 0003:Job finished"))
            {
                string[] words = s.Split(' ');
                int pagesNum = convertStringToInt(words[7].Replace(",", ""));

                Console.WriteLine("pagesNum: " + pagesNum);
                return pagesNum;
            }
            else return 0;
        }

        // Catching error
        private bool error(string s)
        {
            // if this error occurs then return no error because this error is shown multiple times when theres no error
            if (s.Contains("Fatal error 0030:Nothing to print.")) return false;
            if (s.Contains("error")) return true;
            return false;
        }

        // Catching error message
        private string errorMessage(string s)
        {
            // if this error occurs then return null because this error is shown multiple times when theres no error
            if (s.Contains("Fatal error 0030:Nothing to print.")) return null;
            if (s.Contains("error"))
            {
                return s;
            }
            return null;
        }

        // trimming time to readable format from e.g. 19:30:20.289 to 19:30:20
        private string trimTime(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        private int convertStringToInt(string sheets)
        {
            return Int32.Parse(sheets);
        }

        // Getting log from reference
        public string getLog()
        {
            if (log != null)
            {
                return log.ToString();
            }
            else return "Some error occur";            
        }

        
        private class MyLog
        {
            public string startTime;
            private string completeRunTime;
            private string country;
            private int sheetsNumber;
            private bool isB2B;
            private bool isB2C;
            private bool isEC;
            private bool isError;
            private string errorMessage;

            public MyLog(){}

            public void setStartTime(string s)
            {
                this.startTime = s;
            }

            public string getStartTime()
            {
                return this.startTime;
            }

            public void setCompleteRunTime(string s)
            {
                this.completeRunTime = s;
            }

            public string getCompleteRunTime()
            {
                return this.completeRunTime;
            }

            public void setCountry(string s)
            {
                this.country = s;
            }

            public string getCountry()
            {
                return this.country;
            }

            public void setSheetsNumber(int i)
            {
                this.sheetsNumber = i;
            }

            public int getSheetsNumber()
            {
                return this.sheetsNumber;
            }

            public void setIsError(bool b)
            {
                this.isError = b;
            }

            public bool getIsError()
            {
                return this.isError;
            }

            public void setErrorMessage(string s)
            {
                this.errorMessage = s;
            }

            public string getErrorMessage()
            {
                return this.errorMessage;
            }
        }
    }
}
