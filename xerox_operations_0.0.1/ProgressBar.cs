using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xerox_operations
{
    public partial class ProgressBar : Form
    {
        public ProgressBar()
        {
            InitializeComponent();
            Load += ProgressBar_ChangeSize;
            this.timer1.Start();

        }

        void ProgressBar_ChangeSize(object sender, EventArgs e)
        {
            Location = new Point(1250, 400);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {            
            this.progressBar1.Increment(2);
            if (progressBar1.Value == 100) this.Close();
        }

        public void setCheck_1(string s)
        {
            this.label1.Text = s;
        }

        public void setCheck_2(string s)
        {
            this.label1.Text = s;
        }

        public void setCheck_3(string s)
        {
            this.label1.Text = s;
        }
    }

}
