using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xerox_operations_0._0._1.datacard
{
   public class Datacard
    {
        private string uniquePrinterNumber;
        private string ip;
        private string name;

        private long billing;
        private int ribbonRemaining;
        private string status;
        private bool isOnline;
        private string version;
        private bool isLocked;
        private int printerMessageNumber;

        public Datacard(string name, string ip, string uniqueNumber)
        {
            this.name = name;
            this.ip = ip;
            this.uniquePrinterNumber = uniqueNumber;
        }

        public string getIp()
        {
            return this.ip;
        }

        public string getName()
        {
            return this.name;
        }

        public string getUniquePrinterNumber()
        {
            return this.uniquePrinterNumber;
        }    

        public void setBilling(long billing)
        {
            this.billing = billing;
        }

        public long getBilling()
        {
            return this.billing;
        }
        
        public void setRibbonRemaining(int ribbonRemaining)
        {
            this.ribbonRemaining = ribbonRemaining;
        }

        public int getRibbonRemaining()
        {
            return this.ribbonRemaining;
        }
        
        public void setStatus(string status)
        {
            this.status = status;
        }

        public string getStatus()
        {
            return this.status;
        }

        public void setIsOnline(bool isOnline)
        {
            this.isOnline = isOnline;
        }

        public bool getIsOnline()
        {
            return isOnline;
        }

        public void setVersion(string version)
        {
            this.version = version;
        }

        public string getVersion()
        {
            return this.version;
        }

        public void setLocked(bool isLocked)
        {
            this.isLocked = isLocked;
        }

        public bool getLocked()
        {
            return this.isLocked;
        }                

        public void setPrinterMessageNumber(int number)
        {
            this.printerMessageNumber = number;
        }

        public int getPrinterMessageNumber()
        {
            return this.printerMessageNumber;
        }
    }
}
