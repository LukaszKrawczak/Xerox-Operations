using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using xerox_operations.values;
using xerox_operations_0._0._1.datacard;
using xerox_operations_0._0._1.utils;
using xerox_operations.window;
using xerox_operations.utils;

namespace xerox_operations_0._0._1
{
    /// This class represents Main View of program with all controls and units.
    public partial class MainForm : Form
    {
        private DirectoryMonitor b2b;
        private DirectoryMonitor b2c;
        private Printer nv1;
        private Printer nv2;
        private Printer nv3;
        private Printer nv4;
        private Printer phaser;
        private LaczeV5Log log;
        ToolTip tip = new ToolTip();
        private string timeStamp = string.Format("{0:yyyy-MM-dd hh:mm:ss}", DateTime.Now);

        public MainForm()
        {
            InitializeComponent();
            b2b = new DirectoryMonitor(this, Paths.B2B_PATH);
            b2c = new DirectoryMonitor(this, Paths.B2C_PATH);
            initPrinters();
            initDataCards();
            Load += MainForm_ChangeSize;
            new Clock(this);
 
            FileCounter fileCounterStoreTno = new FileCounter(this, Paths.STORE_TNO);
            fileCounterStoreTno.loadViewStoreTnoDelayed();
            
            FileCounter fileCounterMaterials = new FileCounter(this, Paths.MATERIALS);
            fileCounterMaterials.loadViewMaterialsDelayed();

            log = new LaczeV5Log(this);
        }

        // Creating printer services on different threads.
        private void initPrinters()
        {
            nv1 = new Printer("nv1", Printers.IP_NV1);
            PrinterStats psNv1 = new PrinterStats(this, nv1);
            Thread thread_1 = new Thread(psNv1.getPrinterStatus);
            thread_1.Start();

            nv2 = new Printer("nv2", Printers.IP_NV2);
            PrinterStats psNv2 = new PrinterStats(this, nv2);
            Thread thread_2 = new Thread(psNv2.getPrinterStatus);
            thread_2.Start();

            nv3 = new Printer("nv3", Printers.IP_NV3);
            PrinterStats psNv3 = new PrinterStats(this, nv3);
            Thread thread_3 = new Thread(psNv3.getPrinterStatus);
            thread_3.Start();

            nv4 = new Printer("nv4", Printers.IP_NV4);
            PrinterStats psNv4 = new PrinterStats(this, nv4);
            Thread thread_4 = new Thread(psNv4.getPrinterStatus);
            thread_4.Start();

            phaser = new Printer("Phaser", Printers.IP_PHASER);
            PrinterStats psPhaser = new PrinterStats(this, phaser);
            Thread thread_5 = new Thread(psPhaser.getPrinterStatus);
            thread_5.Start();
        }

        // Creating Datacards services on different threads.
        private void initDataCards()
        {
            Datacard dc2 = new Datacard("CD800-2", Datacards.IP_DC2, Datacards.NumID_SM_DC2);
            DatacardStats datacardStats2 = new DatacardStats(this, dc2);
            Thread thread_7 = new Thread(datacardStats2.getDatacardStatus);
            thread_7.Start();

            Datacard dc3 = new Datacard("CD800-3", Datacards.IP_DC3, Datacards.NumID_SM_DC3);
            DatacardStats datacardStats3 = new DatacardStats(this, dc3);
            Thread thread_8 = new Thread(datacardStats3.getDatacardStatus);
            thread_8.Start();

            Datacard dc4 = new Datacard("CD800-4", Datacards.IP_DC4, Datacards.NumID_SM_DC4);
            DatacardStats datacardStats4 = new DatacardStats(this, dc4);
            Thread thread_9 = new Thread(datacardStats4.getDatacardStatus);
            thread_9.Start();

            Datacard dc5 = new Datacard("CD800-5", Datacards.IP_DC5, Datacards.NumID_SM_DC5);
            DatacardStats datacardStats5 = new DatacardStats(this, dc5);
            Thread thread_10 = new Thread(datacardStats5.getDatacardStatus);
            thread_10.Start();

            Datacard dc6 = new Datacard("CD800-6", Datacards.IP_DC6, Datacards.NumID_SM_DC6);
            DatacardStats datacardStats6 = new DatacardStats(this, dc6);
            Thread thread_11 = new Thread(datacardStats6.getDatacardStatus);
            thread_11.Start();

            Datacard dc7 = new Datacard("CD800-7", Datacards.IP_DC7, Datacards.NumID_SM_DC7);
            DatacardStats datacardStats7 = new DatacardStats(this, dc7);
            Thread thread_12 = new Thread(datacardStats7.getDatacardStatus);
            thread_12.Start();
        }

        // Poiting window to new Location at startup
        void MainForm_ChangeSize(object sender, EventArgs e)
        {
            Location = new Point(880, 0);
        }

        // Button responsible for cleaning log.
        private void buttonClearLog_Click(object sender, EventArgs e)
        {
            b2c.clearLog();
            richTextBoxB2C_TextChanged(sender, e);
            b2b.clearLog();
            richTextBoxB2B_TextChanged(sender, e);
        }

        // Richbox B2B textChanger. This method add text and Scrolling down to the end.
        public void richTextBoxB2B_TextChanged(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                this.richTextBoxB2B.Text = b2b.getFileName();
                this.richTextBoxB2B.SelectionStart = this.richTextBoxB2B.Text.Length;
                this.richTextBoxB2B.ScrollToCaret();
            });
        }

        // Richbox B2C textChanger. This method add text and Scrolling down to the end.
        public void richTextBoxB2C_TextChanged(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                this.richTextBoxB2C.Text = b2c.getFileName();
                this.richTextBoxB2C.SelectionStart = this.richTextBoxB2C.Text.Length;
                this.richTextBoxB2C.ScrollToCaret();
            });
        }

        // *********** START PRINTERS *********** // 
        // Nuvera144-1
        // Nuvera 1
        // This methods will be responsible for updating view.
        public void smNv1LabelChanged(string value)
        {
            InvokeLabelFromThread(smNuvera1Label, value);
        }

        // When status changes, this indicator updates itself
        public void smNv1IndicatorChanged(int value)
        {
            changeStatus(this.smNv1StatusIndicator, value);
        }

        // When there is low paper in one tray
        public void smNv1LowPaperIndicator1Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smNv1LowPaperIndicator1, isEmpty);
        }

        // When there is low paper in more that one tray
        public void smNv1LowPaperIndicator2Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smNv1LowPaperIndicator2, isEmpty);
        }

        // When FinisherA finished. 
        public void smNv1FinishedStackerA(bool isFinished)
        {
            setVisibilityIndicator(this.smNv1FinishIndicator1, isFinished);
        }

        // When FinisherB finished.
        public void smNv1FinishedStackerB(bool isFinished)
        {
            setVisibilityIndicator(this.smNv1FinishIndicator2, isFinished);
        }

        // When cursor poiting on StatusIndicator icon.
        private void smNv1StatusIndicator_MouseHover(object sender, EventArgs e)
        {
            addItemsToListView(nv1.messages);
        }

        // When cursor leaves from StatusIndicator icon.
        private void smNv1StatusIndicator_MouseLeave(object sender, EventArgs e)
        {
            clearPrinterStatusListView();
        }

        // Nuvera144-2
        // This methods will be responsible for updating view.
        public void smNv2LabelChanged(string value)
        {
            InvokeLabelFromThread(smNuvera2Label, value);
        }

        // When status changes, this indicator updates itself
        public void smNv2IndicatorChanged(int value)
        {
            changeStatus(this.smNv2StatusIndicator, value);
        }

        // When there is low paper in one tray
        public void smNv2LowPaperIndicator1Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smNv2LowPaperIndicator1, isEmpty);
        }

        // When there is low paper in more that one tray
        public void smNv2LowPaperIndicator2Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smNv2LowPaperIndicator2, isEmpty);
        }

        // When FinisherA finished. 
        public void smNv2FinishedStackerA(bool isFinished)
        {
            setVisibilityIndicator(this.smNv2FinishIndicator1, isFinished);
        }

        // When FinisherB finished.
        public void smNv2FinishedStackerB(bool isFinished)
        {
            setVisibilityIndicator(this.smNv2FinishIndicator2, isFinished);
        }

        // When cursor poiting on StatusIndicator icon.
        private void smNv2StatusIndicator_MouseHover(object sender, EventArgs e)
        {
            addItemsToListView(nv2.messages);
        }

        // When cursor leaves from StatusIndicator icon.
        private void smNv2StatusIndicator_MouseLeave(object sender, EventArgs e)
        {
            clearPrinterStatusListView();
        }

        // Nuvera144-3
        // This methods will be responsible for updating view.
        public void smNv3LabelChanged(string value)
        {
            InvokeLabelFromThread(smNuvera3Label, value);
        }

        // When status changes, this indicator updates itself
        public void smNv3IndicatorChanged(int value)
        {
            changeStatus(this.smNv3StatusIndicator, value);
        }

        // When there is low paper in one tray
        public void smNv3LowPaperIndicator1Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smNv3LowPaperIndicator1, isEmpty);
        }

        // When there is low paper in more that one tray
        public void smNv3LowPaperIndicator2Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smNv3LowPaperIndicator2, isEmpty);
        }

        // When FinisherA finished. 
        public void smNv3FinishedStackerA(bool isFinished)
        {
            setVisibilityIndicator(this.smNv3FinishIndicator1, isFinished);
        }

        // When FinisherB finished.
        public void smNv3FinishedStackerB(bool isFinished)
        {
            setVisibilityIndicator(this.smNv3FinishIndicator2, isFinished);
        }

        // When cursor poiting on StatusIndicator icon.
        private void smNv3StatusIndicator_MouseHover(object sender, EventArgs e)
        {
            addItemsToListView(nv3.messages);
        }

        // When cursor leaves from StatusIndicator icon.
        private void smNv3StatusIndicator_MouseLeave(object sender, EventArgs e)
        {
            clearPrinterStatusListView();
        }


        // Nuvera144-4
        // This methods will be responsible for updating view.
        public void smNv4LabelChanged(string value)
        {
            InvokeLabelFromThread(this.smNuvera4Label, value);
        }

        // When status changes, this indicator updates itself
        public void smNv4IndicatorChanged(int value)
        {
            changeStatus(this.smNv4StatusIndicator, value);
        }

        // When there is low paper in one tray
        public void smNv4LowPaperIndicator1Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smNv4LowPaperIndicator1, isEmpty);
        }

        // When there is low paper in more that one tray
        public void smNv4LowPaperIndicator2Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smNv4LowPaperIndicator2, isEmpty);
        }

        // When FinisherA finished. 
        public void smNv4FinishedStackerA(bool isFinished)
        {
            setVisibilityIndicator(this.smNv4FinishIndicator1, isFinished);
        }

        // When FinisherB finished.
        public void smNv4FinishedStackerB(bool isFinished)
        {
            setVisibilityIndicator(this.smNv4FinishIndicator2, isFinished);
        }

        // When cursor poiting on StatusIndicator icon.
        private void smNv4StatusIndicator_MouseHover(object sender, EventArgs e)
        {
            addItemsToListView(nv4.messages);
        }

        // When cursor leaves from StatusIndicator icon.
        private void smNv4StatusIndicator_MouseLeave(object sender, EventArgs e)
        {
            clearPrinterStatusListView();
        }

        // Phaser 5550
        // This methods will be responsible for updating view.
        public void smPhaserLabelChanged(string value)
        {
            InvokeLabelFromThread(this.smPhaserLabel, value);
        }

        // When status changes, this indicator updates itself
        public void smPhaserIndicatorChanged(int value)
        {
            changeStatus(this.smPhaserStatusIndicator, value);
        }

        // When there is low paper in one tray
        public void smPhaserLowPaperIndicator1Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smPhaserLowPaperIndicator1, isEmpty);
        }

        // When there is low paper in more that one tray
        public void smPhaserLowPaperIndicator2Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smPhaserLowPaperIndicator2, isEmpty);
        }

        // When cursor poiting on StatusIndicator icon.
        private void smPhaserStatusIndicator_MouseHover(object sender, EventArgs e)
        {
            addItemsToListView(phaser.messages);
        }

        // When cursor leaves from StatusIndicator icon.
        private void smPhaserStatusIndicator_MouseLeave(object sender, EventArgs e)
        {
            clearPrinterStatusListView();
        }

        // *********** END PRINTERS *********** // 

        // *********** START DATACARDS *********** // 
        // Datacard 2
        // Updates percentage of ribbon
        public void dc2_ribbonLabelChanged(int value)
        {
            InvokeLabelFromThread(this.dc2RibbonLabel, value.ToString() + "%");
        }

        // Shows actual billing state
        public void dc2_billingLabelChanged(long value)
        {
            InvokeLabelFromThread(this.dc2BillingLabel, value.ToString());
        }

        // Actual status of printer
        public void dc2_StatusIndicatorChanged(int value)
        {
            changeStatus(this.dc2StatusIndicator, value);
        }

        // Shows if printer is locked by user.
        public void dc2_LockIndicatorChanged(bool isLocked)
        {
            setVisibilityIndicator(this.dc2LockIndicator, isLocked);
        }

        // Datacard 3
        // Updates percentage of ribbon
        public void dc3_ribbonLabelChanged(int value)
        {
            InvokeLabelFromThread(this.dc3RibbonLabel, value.ToString() + "%");
        }

        // Shows actual billing state
        public void dc3_billingLabelChanged(long value)
        {
            InvokeLabelFromThread(this.dc3BillingLabel, value.ToString());
        }

        // Actual status of printer
        public void dc3_StatusIndicatorChanged(int value)
        {
            changeStatus(this.dc3StatusIndicator, value);
        }

        // Shows if printer is locked by user.
        public void dc3_LockIndicatorChanged(bool isLocked)
        {
            setVisibilityIndicator(this.dc3LockIndicator, isLocked);
        }

        // Datacard 4
        // Updates percentage of ribbon
        public void dc4_ribbonLabelChanged(int value)
        {
            InvokeLabelFromThread(this.dc4RibbonLabel, value.ToString() + "%");
        }

        // Shows actual billing state
        public void dc4_billingLabelChanged(long value)
        {
            InvokeLabelFromThread(this.dc4BillingLabel, value.ToString());
        }

        // Actual status of printer
        public void dc4_StatusIndicatorChanged(int value)
        {
            changeStatus(this.dc4StatusIndicator, value);
        }

        // Shows if printer is locked by user.
        public void dc4_LockIndicatorChanged(bool isLocked)
        {
            setVisibilityIndicator(this.dc4LockIndicator, isLocked);
        }

        // Datacard 5
        // Updates percentage of ribbon
        public void dc5_ribbonLabelChanged(int value)
        {
            InvokeLabelFromThread(this.dc5RibbonLabel, value.ToString() + "%");
        }

        // Shows actual billing state
        public void dc5_billingLabelChanged(long value)
        {
            InvokeLabelFromThread(this.dc5BillingLabel, value.ToString());
        }

        // Actual status of printer
        public void dc5_StatusIndicatorChanged(int value)
        {
            changeStatus(this.dc5StatusIndicator, value);
        }

        // Shows if printer is locked by user.
        public void dc5_LockIndicatorChanged(bool isLocked)
        {
            setVisibilityIndicator(this.dc5LockIndicator, isLocked);
        }

        // Datacard 6
        // Updates percentage of ribbon
        public void dc6_ribbonLabelChanged(int value)
        {
            InvokeLabelFromThread(this.dc6RibbonLabel, value.ToString() + "%");
        }

        // Shows actual billing state
        public void dc6_billingLabelChanged(long value)
        {
            InvokeLabelFromThread(this.dc6BillingLabel, value.ToString());
        }

        // Actual status of printer
        public void dc6_StatusIndicatorChanged(int value)
        {
            changeStatus(this.dc6StatusIndicator, value);
        }

        // Shows if printer is locked by user.
        public void dc6_LockIndicatorChanged(bool isLocked)
        {
            setVisibilityIndicator(this.dc6LockIndicator, isLocked);
        }

        // Datacard 7
        // Updates percentage of ribbon
        public void dc7_ribbonLabelChanged(int value)
        {
            InvokeLabelFromThread(this.dc7RibbonLabel, value.ToString() + "%");
        }

        // Shows actual billing state
        public void dc7_billingLabelChanged(long value)
        {
            InvokeLabelFromThread(this.dc7BillingLabel, value.ToString());            
        }

        // Actual status of printer
        public void dc7_StatusIndicatorChanged(int value)
        {
            changeStatus(this.dc7StatusIndicator, value);
        }

        // Shows if printer is locked by user.
        public void dc7_LockIndicatorChanged(bool isLocked)
        {
            setVisibilityIndicator(this.dc7LockIndicator, isLocked);
        }

        // *********** END DATACARDS *********** // 

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Point locationOnForm = smNv1StatusIndicator.FindForm().PointToClient(
            smNv1StatusIndicator.Parent.PointToScreen(smNv1StatusIndicator.Location));
            Console.WriteLine(locationOnForm);

            string s = "Łukasz Krawczak";
            //var listViewItem = new ListViewItem(s);
            //printerStatusListView.Items.Add(listViewItem);
        }

        // Adds items to List View
        private void addItemsToListView(List<string> messages)
        {
            foreach (var item in messages)
            {
                var listViewItem = new ListViewItem(item);
                printerStatusListView.Items.Add(listViewItem);
            }
        }    

        // This method is responsible for cleaning listview
        public void clearPrinterStatusListView()
        {
            printerStatusListView.Items.Clear();
        }

        // This method is responsible for hiding PictureBox from MainForm
        public void setVisibilityIndicator(PictureBox pictureBox, bool isEmpty)
        {
            Invoke((MethodInvoker)delegate
            {
                if (isEmpty) pictureBox.Show();
                else pictureBox.Hide();
            });
        }       

        public void statusIndicatorRed(PictureBox pictureBox)
        {
            pictureBox.Image = Image.FromFile("../Image/icon_indicator_red.png");
        }

        public void statusIndicatorGreen(PictureBox pictureBox)
        {
            pictureBox.Image = Image.FromFile("../Image/icon_indicator_green.png");
        }

        public void statusIndicatorOrange(PictureBox pictureBox)
        {
            pictureBox.Image = Image.FromFile("../Image/icon_indicator_orange.png");
        }

        public void statusIndicatorCircle(PictureBox pictureBox)
        {
            pictureBox.Image = Image.FromFile("../Image/icon_indicator_circle_dark_green.png");
        }

        public void statusIndicatorFail(PictureBox pictureBox)
        {
            pictureBox.Image = Image.FromFile("../Image/icon_indicator_fail.png");
        }

        public void statusIndicatorOK(PictureBox pictureBox)
        {
            pictureBox.Image = Image.FromFile("../Image/icon_done_indicator.png");
        }

        public void changeStatus(PictureBox pictureBox, int status)
        {
            switch (status)
            {
                case 1:
                    statusIndicatorRed(pictureBox);
                    break;
                case 2:
                    statusIndicatorGreen(pictureBox);
                    break;
                case 4:
                    statusIndicatorCircle(pictureBox);
                    break;
                case 5:
                    statusIndicatorOrange(pictureBox);
                    break;
                case 6:
                    statusIndicatorFail(pictureBox);
                    break;
                case 7:
                    statusIndicatorOK(pictureBox);
                    break;
                default:
                    statusIndicatorGreen(pictureBox);
                    break;
            }
        }

        public void onDriveProgressBar(int progress)
        {
            Invoke((MethodInvoker)delegate
            {
                this.progressBarDriveD.Value = progress;
                if (progress > 80)
                {
                    this.progressBarDriveD.ForeColor = Color.Red;
                }
            });
        }

        public void onDriveFreeSpaceAvailableLabelChanged(int progress)
        {
            InvokeLabelFromThread(this.labelDriveD, progress.ToString() + "% wolnego miejsca");
            //Invoke((MethodInvoker)delegate
            //{
            //    this.labelDriveD.Text = progress.ToString() + "% wolnego miejsca";
            //});
        }

        public void onToolStripStatusLabelSessionTimeChanged(string time)
        {
            Invoke((MethodInvoker)delegate
            {
                this.toolStripStatusLabelSessionTime.Text = "Czas sesji: " + time;
            });
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ApplicationController.close();
        }

        // Invoking Text to UI from another Thread.
        private void InvokeLabelFromThread(Label label, string text)
        {
            Invoke((MethodInvoker)delegate
            {
                label.Text = text;
            });
        }
        
        private void showToolTip(string printerName, Point locationOnForm)
        {
            tip.ToolTipTitle = "NV1";
            tip.ToolTipIcon = ToolTipIcon.Info;
            tip.IsBalloon = true;
            tip.Show("hello! \r\nasdasd \r\nblablabla \r\nasdasdasdasd", this, new Point(locationOnForm.X, locationOnForm.Y - 40));
        }

        private void buttonDeleteOrderOrSid_Click(object sender, EventArgs e)
        {
            ButtonController.startAnotherApplication(@"C:\Skrypty\Usprawnienie\Usprawnienie_prod.jar");
        }

        private void buttonBlockCurierLabels_Click(object sender, EventArgs e)
        {
            ButtonController.startAnotherApplication(@"C:\Skrypty\BlokowanieEtykietKurierskich.cmd");
        }

        private void buttonPrintInstalments_Click(object sender, EventArgs e)
        {
            ButtonController.startAnotherApplication(@"C:\Skrypty\Drukowanie instalmentow z wave.bat");
        }

        private void buttonReprintLeaflet_Click(object sender, EventArgs e)
        {
            ButtonController.startAnotherApplication(@"C:\Jonit\wfd\smProd\si\prod-sm-InsertReprint.pl");
        }

        private void buttonReprocessSid_Click(object sender, EventArgs e)
        {
            new ReprocessSid(openFileDialogReprocessSid);
        }

        public void labelFileStoreTnoNames_TextChanged(string value)
        {
            InvokeLabelFromThread(this.labelFileStoreTnoNames, value);
        }

        public void labelFileStoreTnoValues_TextChanged(string value)
        {
            InvokeLabelFromThread(this.labelFileStoreTnoCounter, value);
        }

        public void onFileStoreTnoCounterIndicator_Changed(int value)
        {
            changeStatus(this.fileStoreTnoCounterIndicator, value);
        }

        public void labelFileMaterialsNames_TextChanged(string value)
        {
            InvokeLabelFromThread(this.labelFileMaterialsNames, value);
        }

        public void labelFileMaterialsValues_TextChanged(string value)
        {
            InvokeLabelFromThread(this.labelFileMaterialsCounter, value);
        }

        public void onMaterialsPreprocessIndicator_Changed(int value)
        {
            changeStatus(this.materialsPreprocessedIndicator, value);
        }

        //labelGlassfishStatus
        public void onGlassfishStatusLabelChanged(string status)
        {
            InvokeLabelFromThread(this.labelGlassfishStatus, status.ToString());
        }

        public void onGlassfishStatusIndicator_Changed(int value)
        {
            changeStatus(this.glassfishStatusIndicator, value);
        }

        private void fileMaterialsPanel_Click(object sender, EventArgs e)
        {
            ButtonController.startAnotherApplication(@"D:\Jonit_VAR\Prod\msgSM-X_Materials");
        }

        private void labelFileMaterialsNames_Click(object sender, EventArgs e)
        {
            ButtonController.startAnotherApplication(@"D:\Jonit_VAR\Prod\msgSM-X_Materials");
        }

        public void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                this.richTextBox1.Text = log.getLog();
                this.richTextBox1.SelectionStart = this.richTextBox1.Text.Length;
                this.richTextBox1.ScrollToCaret();
            });
        }
    }
}
