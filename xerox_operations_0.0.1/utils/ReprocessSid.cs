using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using xerox_operations_0._0._1;

namespace xerox_operations.utils
{
    public class ReprocessSid
    {
        private OpenFileDialog openFileDialog;
        private ProgressBar progressBar;

        private System.Windows.Forms.Timer myTimer;
        private long time;

        string targetPath = @"D:\Jonit_VAR\Prod\msgDHL-X_Wave\";
        string check1 = @"Przenoszę plik do folderu \Jonit_VAR\Prod\msgDHL-X_Wave";
        string check2 = @"Plik znajduje się w folderze";
        string check3 = @"Przetwarzam plik";
        string file;

        bool isMoved; 
        bool isProccessed;
        
        public ReprocessSid(OpenFileDialog openFileDialog)
        {
            this.openFileDialog = openFileDialog;
            openDialog();
        }

        /// <summary>
        /// Opening Dialog where user can select file to process
        /// </summary>
        private void openDialog()
        {
            openFileDialog.ShowDialog();
            file = openFileDialog.FileName;

            try
            {
                System.IO.File.Move(file, targetPath + Path.GetFileName(file));
                progressBar = new ProgressBar();               
                progressBar.Show();

                myTimer = new System.Windows.Forms.Timer();
                myTimer.Tick += new EventHandler(OnTick);
                myTimer.Interval = 1000;
                myTimer.Start();
                Console.WriteLine("openDialog: "+time);
            }
            catch (Exception ex)
            {                
            }                      
        }

        /// <summary>
        /// Thicking every 1 second.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTick(object sender, EventArgs e)
        {
            time += 1000;
            if (!isMoved) moveFile();
            if (!isProccessed) processFile();
            if (isProccessed && isMoved) myTimer.Stop();
        }

        /// <summary>
        /// Moving file to source directory
        /// </summary>
        private void moveFile()
        {           
            if (time == 1000) progressBar.setCheck_1(check1);
            if (File.Exists(targetPath + Path.GetFileName(file)))
            {
                if (time == 2000)
                {
                    progressBar.setCheck_2(check2);
                    isMoved = true;
                }
            }
        }
        
        /// <summary>
        /// Starting another application to process the Wave
        /// </summary>
        private void processFile()
        {
            if (time == 5000)
            {
                progressBar.setCheck_3(check3);
                ButtonController.startAnotherApplication(@"C:\Jonit\wfd\smProd\si\prod-sm-preprocessWave.pl");
                isProccessed = true;
                time = 0;
            }
        }
    }
}
