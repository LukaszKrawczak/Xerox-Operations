using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace xerox_operations_0._0._1.utils
{
    class Clock
    {
        private MainForm mainForm;

        private int seconds;
        private int minutes;
        private int hours;

        public Clock(MainForm mainForm)
        {
            this.mainForm = mainForm;
            startTicking();
        }

        private void startTicking()
        {
            // Set up a timer that triggers every minute.
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 1000; // 60 seconds
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            seconds++;

            if (seconds == 60)
            {
                seconds = 0;
                minutes++;
            }

            if (minutes == 60)
            {
                minutes = 0;
                hours++;
            }
           
            showTimer(addZeroIfNeeded(hours), addZeroIfNeeded(minutes), addZeroIfNeeded(seconds));
        }

        private string addZeroIfNeeded(int i)
        {
            if (i.ToString().Length == 1) return "0" + i.ToString();
            else return i.ToString();
        }

        private void showTimer(string hours, string minutes, string seconds)
        {
            string s = hours.ToString() + ":" + minutes.ToString() + ":" + seconds.ToString();
            this.mainForm.onToolStripStatusLabelSessionTimeChanged(s);
        }
    }
}
