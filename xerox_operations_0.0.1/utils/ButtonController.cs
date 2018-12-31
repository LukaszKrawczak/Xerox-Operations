using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xerox_operations.utils
{
    public static class ButtonController
    {
        public static void startAnotherApplication(string uri)
        {
            try
            {
                System.Diagnostics.Process.Start(uri);
            }
            catch
            {
                MessageBox.Show("Nie można odnaleźć określonego pliku", "Błąd");
            }
        }
    }
}
