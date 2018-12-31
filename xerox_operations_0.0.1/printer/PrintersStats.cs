using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xerox_operations_0._0._1
{

    class PrinterStats
    {
        private MainForm mainForm;
        private Printer printer;

        private List<string> messagesTemp;

        public readonly static int REFRESH_TIME = 5000;

        string OID_Billing = "1.3.6.1.2.1.43.10.2.1.4.1.1";
        string OID_SerialNo = "1.3.6.1.2.1.43.5.1.1.17.1";
        string OID_Status = "1.3.6.1.2.1.25.3.5.1.1.1";

        string OID_Message1 = "1.3.6.1.2.1.43.16.5.1.2.1.1";
        string OID_Message2 = "1.3.6.1.2.1.43.16.5.1.2.1.2";
        string OID_Message3 = "1.3.6.1.2.1.43.16.5.1.2.1.3";
        string OID_Message4 = "1.3.6.1.2.1.43.16.5.1.2.1.4";
        string OID_Message5 = "1.3.6.1.2.1.43.16.5.1.2.1.5";
        string OID_Message6 = "1.3.6.1.2.1.43.16.5.1.2.1.6";
        string OID_Message7 = "1.3.6.1.2.1.43.16.5.1.2.1.7";
        string OID_Message8 = "1.3.6.1.2.1.43.16.5.1.2.1.8";
        string OID_Message9 = "1.3.6.1.2.1.43.16.5.1.2.1.9";

        public PrinterStats(MainForm mainForm, Printer printer)
        {
            this.mainForm = mainForm;
            this.printer = printer;

            messagesTemp = new List<string>();
            printer.isOnline = isConnected();

        }

        /*
        * Checking if printer is printer is connected
        */
        private bool isConnected()
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(printer.ip);

                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (PingException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private void configurePrinter()
        {
            try
            {
                // SNMP community name
                OctetString community = new OctetString("public");

                // Define agent parameters class
                AgentParameters param = new AgentParameters(community);

                // Set SNMP version to 1 (or 2)
                param.Version = SnmpVersion.Ver1;

                // Construct the agent address object
                // IpAddress class is easy to use here because
                // it will try to resolve constructor parameter if it doesn't
                // parse to an IP address
                IpAddress ipAddress = new IpAddress(printer.ip);

                // Construct target
                UdpTarget target = new UdpTarget((IPAddress)ipAddress, 161, 2000, 1);

                setCheckerPrinter(param, target, OID_SerialNo);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                throw new ArgumentException("It seems, like there is entered a invalid IP Address");
            }
        }

        /*
         * Fetching informations from Printer.
         */
        private void setCheckerPrinter(AgentParameters param, UdpTarget target, string s)
        {
            try
            {
                // Pdu class used for all requests
                Pdu pdu = new Pdu(PduType.Get);
                pdu.VbList.Add(s);

                // Make SNMP request
                SnmpV1Packet result = (SnmpV1Packet)target.Request(pdu, param);

                printer.uniquePrinterNumber = result.Pdu.VbList[0].Value.ToString();

                printer.setBilling(getBilling(param, target, OID_Billing));
                addMessagesToList(param, target);
                printer.setStatus(getStatus(param, target, OID_Status));

                //Console.WriteLine(getMessages(param, target, OID_Message5).Equals("Null"));
                //printer.setMessage(getMessages(param,target,OID_Message1));
                //Console.WriteLine(printer.getName() + " message: " + getMessages(param, target, OID_Message3));
            }
            catch (SnmpException e)
            {
                printer.setBilling("0");
                Console.WriteLine(e.Message);
            }
        }

        private void addMessagesToList(AgentParameters param, UdpTarget target)
        {
            if (!getMessages(param, target, OID_Message1).Equals("Null"))
            {
                messagesTemp.Add(getMessages(param, target, OID_Message1));
            }
            if (!getMessages(param, target, OID_Message2).Equals("Null"))
            {
                messagesTemp.Add(getMessages(param, target, OID_Message2));
            }
            if (!getMessages(param, target, OID_Message3).Equals("Null"))
            {
                messagesTemp.Add(getMessages(param, target, OID_Message3));
            }
            if (!getMessages(param, target, OID_Message4).Equals("Null"))
            {
                messagesTemp.Add(getMessages(param, target, OID_Message4));
            }
            if (!getMessages(param, target, OID_Message5).Equals("Null"))
            {
                messagesTemp.Add(getMessages(param, target, OID_Message5));
            }
            if (!getMessages(param, target, OID_Message6).Equals("Null"))
            {
                messagesTemp.Add(getMessages(param, target, OID_Message6));
            }
            if (!getMessages(param, target, OID_Message7).Equals("Null"))
            {
                messagesTemp.Add(getMessages(param, target, OID_Message7));
            }
            if (!getMessages(param, target, OID_Message8).Equals("Null"))
            {
                messagesTemp.Add(getMessages(param, target, OID_Message8));
            }
            if (!getMessages(param, target, OID_Message9).Equals("Null"))
            {
                messagesTemp.Add(getMessages(param, target, OID_Message9));
            }
        }

        /**
         *  Preparing informations for labeling.
         */
        private string getBilling(AgentParameters param, UdpTarget target, string OID_Billing)
        {
            // Pdu class used for all requests
            Pdu pdu = new Pdu(PduType.Get);
            pdu.VbList.Add(OID_Billing);

            // Make SNMP request
            SnmpV1Packet result = (SnmpV1Packet)target.Request(pdu, param);

            long value = 0;

            try
            {
                value = long.Parse(result.Pdu.VbList[0].Value.ToString());
            }
            catch(OverflowException e)
            {
                Console.WriteLine(e.Message);
            }

            printer.setBilling(value.ToString("#,#", CultureInfo.InvariantCulture));

            return value.ToString("#,#", CultureInfo.InvariantCulture);
        }

        private string getMessages(AgentParameters param, UdpTarget target, string OID_Message)
        {
            // Pdu class used for all requests
            Pdu pdu = new Pdu(PduType.Get);
            pdu.VbList.Add(OID_Message);

            // Make SNMP request
            SnmpV1Packet result = (SnmpV1Packet)target.Request(pdu, param);

            string value = result.Pdu.VbList[0].Value.ToString();

            return value.ToString();
        }

        private int getStatus(AgentParameters param, UdpTarget target, string OID_Status)
        {
            // Pdu class used for all requests
            Pdu pdu = new Pdu(PduType.Get);
            pdu.VbList.Add(OID_Status);

            // Make SNMP request
            SnmpV1Packet result = (SnmpV1Packet)target.Request(pdu, param);

            string value = result.Pdu.VbList[0].Value.ToString();

            try
            {
                return Int32.Parse(value);
            }
            catch (FormatException e)
            {
                throw new FormatException(e.Message);                
            }
        }

        /**
         * Refreshing 
         */
        public void getPrinterStatus()
        {
            while (true)
            {
                Thread.Sleep(REFRESH_TIME);
                configurePrinter();
                //Console.WriteLine(printer.getName() + " billing: " + printer.getBilling() + " messagesNumber: " + printer.messages.Count());
                //foreach (object o in printer.messages)
                //{
                //    Console.WriteLine(o);
                //}

                printer.messages = new List<string>();
                printer.messages.AddRange(messagesTemp);

                int i = 0;
                foreach (object o in printer.messages)
                {
                    if (Regex.IsMatch(o.ToString(), @"Add Stock to Tray") || Regex.IsMatch(o.ToString(), @"Paper Is Low"))
                    {
                        i++;
                    }     
                }

                switch (i)
                {
                    case 0:
                        printer.setLowPaper1(false);
                        printer.setLowPaper2(false);
                        break;
                    case 1:
                        printer.setLowPaper1(true);
                        printer.setLowPaper2(false);
                        break;
                    default:
                        printer.setLowPaper1(true);
                        printer.setLowPaper2(true);
                        break;                    
                }

                printer.setFinishedStackerA(false);
                printer.setFinishedStackerB(false);
                foreach (object o in printer.messages)
                {
                    if (Regex.IsMatch(o.ToString(), @"Finisher A: Unload Main Stacker")) printer.setFinishedStackerA(true);
                    if (Regex.IsMatch(o.ToString(), @"Finisher B: Unload Main Stacker")) printer.setFinishedStackerB(true);                    
                }


                switch (printer.uniquePrinterNumber)
                {
                    case Printers.NumID_SM_NV1:
                        mainForm.smNv1LabelChanged(printer.getBilling());
                        mainForm.smNv1IndicatorChanged(printer.getStatus());
                        mainForm.smNv1LowPaperIndicator1Changed(printer.getLowPaper1());
                        mainForm.smNv1LowPaperIndicator2Changed(printer.getLowPaper2());
                        mainForm.smNv1FinishedStackerA(printer.getStackerA());
                        mainForm.smNv1FinishedStackerB(printer.getStackerB());
                        break;

                    case Printers.NumID_SM_NV2:
                        mainForm.smNv2LabelChanged(printer.getBilling());
                        mainForm.smNv2IndicatorChanged(printer.getStatus());
                        mainForm.smNv2LowPaperIndicator1Changed(printer.getLowPaper1());
                        mainForm.smNv2LowPaperIndicator2Changed(printer.getLowPaper2());
                        mainForm.smNv2FinishedStackerA(printer.getStackerA());
                        mainForm.smNv2FinishedStackerB(printer.getStackerB());
                        break;

                    case Printers.NumID_SM_NV3:
                        mainForm.smNv3LabelChanged(printer.getBilling());
                        mainForm.smNv3IndicatorChanged(printer.getStatus());
                        mainForm.smNv3LowPaperIndicator1Changed(printer.getLowPaper1());
                        mainForm.smNv3LowPaperIndicator2Changed(printer.getLowPaper2());
                        mainForm.smNv3FinishedStackerA(printer.getStackerA());
                        mainForm.smNv3FinishedStackerB(printer.getStackerB());
                        break;

                    case Printers.NumID_SM_NV4:
                        mainForm.smNv4LabelChanged(printer.getBilling());
                        mainForm.smNv4IndicatorChanged(printer.getStatus());
                        mainForm.smNv4LowPaperIndicator1Changed(printer.getLowPaper1());
                        mainForm.smNv4LowPaperIndicator2Changed(printer.getLowPaper2());
                        mainForm.smNv4FinishedStackerA(printer.getStackerA());
                        mainForm.smNv4FinishedStackerB(printer.getStackerB());
                        break;

                    case Printers.NumID_SM_PHASER:
                        mainForm.smPhaserLabelChanged(printer.getBilling());
                        mainForm.smPhaserIndicatorChanged(printer.getStatus());
                        mainForm.smPhaserLowPaperIndicator1Changed(printer.getLowPaper1());
                        mainForm.smPhaserLowPaperIndicator2Changed(printer.getLowPaper2());
                        break;
                }
                // Preparing for new messages
                messagesTemp.Clear();
            }
        }
    }

}
