using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using dxp01sdk;
using xerox_operations.values;

namespace xerox_operations_0._0._1.datacard
{
    public class DatacardStats
    {
        private MainForm mainForm;
        private Datacard datacard;

        private readonly static int REFRESH_TIME = 3000;
        private readonly static long ONE_MINUTE = 60_000;

        private bool isWorking;
        private bool isLooping;
        
        public DatacardStats(MainForm mainForm, Datacard datacard)
        {
            this.mainForm = mainForm;
            this.datacard = datacard;
            datacard.setStatus("Preparing");
        }

        private bool isConnected()
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(datacard.getIp(),50);
                if (reply.Status == IPStatus.Success) return true;
                else return false;
            }
            catch (PingException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private bool confirmConnection()
        {            
            Thread.Sleep(5000);
            if (isConnected())
            {
                Console.WriteLine(datacard.getName() + " connection confirmed.");
                return true;
            }

            else
            {
                return false;
            }
        }

        public void getDatacardStatus()
        {
            while (true)
            {
                Thread.Sleep(REFRESH_TIME);
                if (isConnected() && !datacard.getStatus().Equals("Shutdown"))
                {
                    getStatus();
                }
                if (!isConnected() || datacard.getStatus().Equals("Shutdown"))
                {
                    Console.WriteLine(datacard.getName() + " DEFAULT VALUES.");
                    datacard.setBilling(0);
                    datacard.setLocked(true);
                    datacard.setRibbonRemaining(0);
                    datacard.setStatus("Unavailable");
                }

                switch(datacard.getUniquePrinterNumber())
                {
                    case Datacards.NumID_SM_DC2:
                        mainForm.dc2_ribbonLabelChanged(datacard.getRibbonRemaining());
                        mainForm.dc2_billingLabelChanged(datacard.getBilling());
                        mainForm.dc2_StatusIndicatorChanged(convertStatus(datacard.getStatus()));
                        //mainForm.dc2_StatusIndicatorChanged(convertStatusWithMessage(datacard.getPrinterMessageNumber()));
                        mainForm.dc2_LockIndicatorChanged(datacard.getLocked());
                        break;

                    case Datacards.NumID_SM_DC3:
                        mainForm.dc3_ribbonLabelChanged(datacard.getRibbonRemaining());
                        mainForm.dc3_billingLabelChanged(datacard.getBilling());
                        mainForm.dc3_StatusIndicatorChanged(convertStatus(datacard.getStatus()));
                        //mainForm.dc3_StatusIndicatorChanged(convertStatusWithMessage(datacard.getPrinterMessageNumber()));
                        mainForm.dc3_LockIndicatorChanged(datacard.getLocked());
                        break;

                    case Datacards.NumID_SM_DC4:
                        mainForm.dc4_ribbonLabelChanged(datacard.getRibbonRemaining());
                        mainForm.dc4_billingLabelChanged(datacard.getBilling());
                        mainForm.dc4_StatusIndicatorChanged(convertStatus(datacard.getStatus()));
                        //mainForm.dc4_StatusIndicatorChanged(convertStatusWithMessage(datacard.getPrinterMessageNumber()));
                        mainForm.dc4_LockIndicatorChanged(datacard.getLocked());
                        break;

                    case Datacards.NumID_SM_DC5:
                        mainForm.dc5_ribbonLabelChanged(datacard.getRibbonRemaining());
                        mainForm.dc5_billingLabelChanged(datacard.getBilling());
                        mainForm.dc5_StatusIndicatorChanged(convertStatus(datacard.getStatus()));
                        //mainForm.dc5_StatusIndicatorChanged(convertStatusWithMessage(datacard.getPrinterMessageNumber()));
                        mainForm.dc5_LockIndicatorChanged(datacard.getLocked());
                        break;

                    case Datacards.NumID_SM_DC6:
                        mainForm.dc6_ribbonLabelChanged(datacard.getRibbonRemaining());
                        mainForm.dc6_billingLabelChanged(datacard.getBilling());
                        mainForm.dc6_StatusIndicatorChanged(convertStatus(datacard.getStatus()));
                        //mainForm.dc6_StatusIndicatorChanged(convertStatusWithMessage(datacard.getPrinterMessageNumber()));
                        mainForm.dc6_LockIndicatorChanged(datacard.getLocked());
                        break;
                }
            }
        }

        private int convertStatus(string s)
        {
            switch (s)
            {
                case "Ready":
                    if(!isLooping) changeWorkingStatus();
                    return 2;

                case "Busy":
                    isWorking = true;
                    return doesWork(isWorking);

                case "Unavailable":
                    return 5;

                case "Shutdown":
                    return 5;
            }
            return 0;
        }

        private int doesWork(bool isWorking)
        {            
            if (isWorking) return 4;
            else return 5;
        }

        private void changeWorkingStatus()
        {
            isLooping = true;
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 15000; // 60 seconds
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("OnTimer");
            isWorking = false;
        }

        private int convertStatusWithMessage(int i)
        {
            switch (i)
            {
                case 102: // Card not in the position.
                    return 1; 
                case 103: // Printer problem.
                    return 1;
                case 104: // Critical problem.
                    return 1;
                case 109: // Print ribbon problem.
                    return 1;
                case 112: // Card hopper empty.
                    return 1;
                case 113: // Close cover to continue.
                    return 1;
                case 114: // Cover opened dring job.
                    return 1;
                case 118: // Print ribbon type problem.
                    return 1;
                case 119: // Print ribbon not supported.
                    return 1;
                case 120: // User paused the printer.
                    return 1;
            }
            return 0;
        }

        private void getStatus()
        {
            string[] args = new string[] { "-n", datacard.getName() };

            CommandLineOptions commandLineOptions = new CommandLineOptions();
            CommandLine.Utility.Arguments arguments = new CommandLine.Utility.Arguments(args);

            if (string.IsNullOrEmpty(arguments["n"])) { Console.WriteLine("Some information"); }
            bool boolVal = false;
            if (Boolean.TryParse(arguments["n"], out boolVal)) { Console.WriteLine("Some information"); }
            commandLineOptions.printerName = arguments["n"];

            if (!string.IsNullOrEmpty(arguments["j"]))
            {
                commandLineOptions.jobStatus = true;
            }

            BidiSplWrap bidiSpl = null;

                try
                {
                    bidiSpl = new BidiSplWrap();
                    bidiSpl.BindDevice(commandLineOptions.printerName);

                    string driverVersionXml = bidiSpl.GetPrinterData(strings.SDK_VERSION);
                    //Console.WriteLine(Environment.NewLine + "driver version: " + Util.ParseDriverVersionXML(driverVersionXml) + Environment.NewLine);

                    string printerOptionsXML = bidiSpl.GetPrinterData(strings.PRINTER_OPTIONS2);
                    PrinterOptionsValues printerOptionsValues = Util.ParsePrinterOptionsXML(printerOptionsXML);
                    DisplayPrinterOptionsValues(printerOptionsValues);

                    string printerCardCountXML = bidiSpl.GetPrinterData(strings.COUNTER_STATUS2);
                    PrinterCounterStatus printerCounterStatusValues = Util.ParsePrinterCounterStatusXML(printerCardCountXML);
                    DisplayPrinterCounterValues(printerCounterStatusValues);

                    string suppliesXML = bidiSpl.GetPrinterData(strings.SUPPLIES_STATUS3);
                    SuppliesValues suppliesValues = Util.ParseSuppliesXML(suppliesXML);
                    DisplaySuppliesValues(suppliesValues);

                    //string printerStatusXML = bidiSpl.GetPrinterData(strings.PRINTER_MESSAGES);
                    //PrinterStatusValues printerStatusValues = Util.ParsePrinterStatusXML(printerStatusXML);
                    //DisplayPrinterStatusValues(printerStatusValues);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    bidiSpl.UnbindDevice();
                }
            
        }



        private void DisplaySuppliesValues(SuppliesValues vals)
        {
            //Console.WriteLine("Supplies:");
            //Console.WriteLine("  RibbonRemaining:            " + vals._ribbonRemaining + "%");
            datacard.setRibbonRemaining(vals._ribbonRemaining);
            //Console.WriteLine();
        }

        private void DisplayPrinterOptionsValues(PrinterOptionsValues vals)
        {
            //datacard.setUniquePrinterNumber(vals._printerSerialNumber);
            if (vals._lockState.Equals("Locked")) datacard.setLocked(false);
            else datacard.setLocked(true);
            datacard.setStatus(vals._printerStatus);
            datacard.setPrinterMessageNumber(vals._printerMessageNumber);

            //Console.WriteLine("Options:");
            //Console.WriteLine("  LockState:                     " + vals._lockState);
            //Console.WriteLine("  PrinterAddress:                " + vals._printerAddress);
            //Console.WriteLine("  PrinterSerialNumber:           " + vals._printerSerialNumber);
            //Console.WriteLine("  PrinterStatus:                 " + vals._printerStatus);
            //Console.WriteLine("  PrinterVersion:                " + vals._printerVersion);
            //Console.WriteLine();
        }

        private void DisplayPrinterCounterValues(PrinterCounterStatus vals)
        {
            datacard.setBilling(vals._currentPicked);

            //Console.WriteLine("Counts:");
            //Console.WriteLine("  CurrentPicked:                 " + vals._currentPicked);            
            //Console.WriteLine();
        }

        private void DisplayPrinterStatusValues(PrinterStatusValues vals)
        {
            Console.WriteLine("Status:");
            Console.WriteLine("  ClientID:      " + vals._clientID);
            Console.WriteLine("  ErrorCode:     " + vals._errorCode);
            Console.WriteLine("  ErrorSeverity: " + vals._errorSeverity);
            Console.WriteLine("  ErrorString:   " + vals._errorString);
            Console.WriteLine("  PrinterData:   " + vals._dataFromPrinter);
            Console.WriteLine("  PrinterJobID:  " + vals._printerJobID);
            Console.WriteLine("  WindowsJobID:  " + vals._windowsJobID);
            Console.WriteLine();
        }
    }
}
