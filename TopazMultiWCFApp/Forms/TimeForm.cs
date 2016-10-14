using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopazMultiWCFApp.Forms
{
    public partial class TimeForm : Form
    {
        TimeSpan time;
        public TimeForm(TimeSpan t)
        {
            InitializeComponent();

            numericUpDown3.Value = t.Seconds;
            numericUpDown2.Value = t.Minutes;
            numericUpDown1.Value = t.Hours;
        }
        public TimeSpan  Time
        {
            get
            {
                return time;
            }
        }
        private void TimeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            time = new TimeSpan((int)numericUpDown1.Value, (int)numericUpDown2.Value, (int)numericUpDown3.Value);
           
        }
    }
}
