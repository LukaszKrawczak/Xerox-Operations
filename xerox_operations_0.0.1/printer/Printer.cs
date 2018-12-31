using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xerox_operations_0._0._1
{
    public class Printer
    {
        private string name;
        public string ip;

        public bool isOnline;
        public string uniquePrinterNumber;
        private string billing;
        public List<string> messages;
        private int status;
        private bool lowPaper1;
        private bool lowPaper2;
        private bool isFinishedStackerA;
        private bool isFinishedStackerB;
        
        public Printer(string name, string printerIP)
        {
            this.name = name;
            this.ip = printerIP;
            this.messages = new List<string>();
        }

        public void setBilling(string s)
        {
            this.billing = s;
        }

        public string getBilling()
        {
            return billing;
        }

        public string getName()
        {
            return name;
        }

        public void setStatus(int i)
        {
            this.status = i;
        }

        public int getStatus()
        {
            return status;
        }

        public void setLowPaper1(bool isLow)
        {
            lowPaper1 = isLow;
        }

        public bool getLowPaper1()
        {
            return lowPaper1;
        }

        public void setLowPaper2(bool isLow)
        {
            lowPaper2 = isLow;
        }

        public bool getLowPaper2()
        {
            return lowPaper2;
        }

        public void setFinishedStackerA(bool isFinished)
        {
            isFinishedStackerA = isFinished;
        }

        public bool getStackerA()
        {
            return isFinishedStackerA;
        }

        public void setFinishedStackerB(bool isFinished)
        {
            isFinishedStackerB = isFinished;
        }

        public bool getStackerB()
        {
            return isFinishedStackerB;
        }






    }
}
