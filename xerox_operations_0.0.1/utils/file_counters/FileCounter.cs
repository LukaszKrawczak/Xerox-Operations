using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using xerox_operations_0._0._1;

namespace xerox_operations.utils
{
    public class FileCounter
    {
        private MainForm mainForm;
        private string directoryPath;

        private FileSystemWatcher fsw;

        private readonly static int REFRESH_TIME = 10000;

        public FileCounter(MainForm f, string directoryPath)
        {
            this.mainForm = f;
            this.directoryPath = directoryPath;

            if (Directory.Exists(directoryPath))
            {
                fsw = new FileSystemWatcher(Path.Combine(Path.GetDirectoryName(directoryPath)));
                fsw.IncludeSubdirectories = true;
                if (directoryPath.Equals(Paths.STORE_TNO)) initFilterStoreTNO();
                if (directoryPath.Equals(Paths.MATERIALS)) initFilterMaterials();
            }
            else MessageBox.Show("Nie znaleziono folderu monitorującego rozmiar plików.", "Błąd");
        }

        private void initFilterMaterials()
        {
            fsw.NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.FileName;
            fsw.Changed += new FileSystemEventHandler(OnChangedMaterials);
            fsw.Created += new FileSystemEventHandler(OnChangedMaterials);
            fsw.Deleted += new FileSystemEventHandler(OnDeletedMaterials);
            fsw.EnableRaisingEvents = true;
        }

        private void OnDeletedMaterials(object sender, FileSystemEventArgs e)
        {
            mainForm.labelFileMaterialsNames_TextChanged(getNameMaterials());
            mainForm.labelFileMaterialsValues_TextChanged(getValuesMaterials());
        }

        private void OnChangedMaterials(object sender, FileSystemEventArgs e)
        {
            mainForm.labelFileMaterialsValues_TextChanged(getValuesMaterials());
        }

        public void loadViewMaterialsDelayed()
        {
            Task.Delay(REFRESH_TIME).ContinueWith(t => OnCreatedMaterials());
        }

        private void OnCreatedMaterials()
        {
            mainForm.labelFileMaterialsNames_TextChanged(getNameMaterials());
            mainForm.labelFileMaterialsValues_TextChanged(getValuesMaterials());
            
        }

        private string getNameMaterials()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(@"Materiały:");
            builder.Append(Environment.NewLine);
            builder.Append("Nie przetworzone: ");
            return builder.ToString();
        }

        private string getValuesMaterials()
        {
            changeMaterialsIcon(getPdfNumber());
            StringBuilder builder = new StringBuilder();
            builder.Append(Environment.NewLine);
            builder.Append(getPdfNumber());
            return builder.ToString();
        }















        private void changeMaterialsIcon(int value)
        {
            if (value > 0) mainForm.onMaterialsPreprocessIndicator_Changed(6);
            else mainForm.onMaterialsPreprocessIndicator_Changed(7);
        }




        private void changeStoreTnoIcon(int value)
        {
            if (value > 0) mainForm.onFileStoreTnoCounterIndicator_Changed(6);
            else mainForm.onFileStoreTnoCounterIndicator_Changed(7);
        }








        private void initFilterStoreTNO()
        {
            fsw.NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.FileName;
            fsw.Changed += new FileSystemEventHandler(OnChangedstoreTNO);
            fsw.Created += new FileSystemEventHandler(OnChangedstoreTNO);
            fsw.Deleted += new FileSystemEventHandler(OnDeletedstoreTNO);
            fsw.EnableRaisingEvents = true;
        }

        public void loadViewStoreTnoDelayed()
        {
            Task.Delay(REFRESH_TIME).ContinueWith(t => OnCreatedStoreTNO());
        }

        private void OnCreatedStoreTNO()
        {
            mainForm.labelFileStoreTnoNames_TextChanged(storeTnoNames());
            mainForm.labelFileStoreTnoValues_TextChanged(storeTnoValues());
        }

        private void OnDeletedstoreTNO(object sender, FileSystemEventArgs e)
        {
            mainForm.labelFileStoreTnoValues_TextChanged(storeTnoValues());
        }

        private void OnChangedstoreTNO(object sender, FileSystemEventArgs e)
        {
            mainForm.labelFileStoreTnoValues_TextChanged(storeTnoValues());
        }

        private string storeTnoNames()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(@"storeTNO:");
            builder.Append(Environment.NewLine);
            builder.Append("Ilość plików w folderze:");
            builder.Append(Environment.NewLine);
            builder.Append("Ilość pustych plików:");
            builder.Append(Environment.NewLine);
            return builder.ToString();
        }

        private string storeTnoValues()
        {
            changeStoreTnoIcon(getEmptyFiles());
            StringBuilder builder = new StringBuilder();
            builder.Append(Environment.NewLine);
            builder.Append(getDirectoryFilesNumber().ToString());
            builder.Append(Environment.NewLine);
            builder.Append(getEmptyFiles().ToString());
            builder.Append(Environment.NewLine);
            return builder.ToString();
        }

        private int getDirectoryFilesNumber()
        {
            int count = Directory.GetFiles(directoryPath, "*", SearchOption.TopDirectoryOnly).Length;
            return count;
        }

        private int getPdfNumber()
        {
            int count = Directory.GetFiles(directoryPath, "*.pdf", SearchOption.TopDirectoryOnly).Length;
            return count;
        }

        private int getEmptyFiles()
        {
            int i = 0;
            DirectoryInfo di = new DirectoryInfo(directoryPath);
            FileInfo[] fiArr = di.GetFiles();

            foreach (FileInfo fi in fiArr)
            {
                if (fi.Length == 0)
                {
                    i++;
                }
            }
            return i;
        }
    }
}
