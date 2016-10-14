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
    public partial class Preferences : Form
    {
        public Preferences()
        {
            InitializeComponent();
            textBox1.Text = Properties.Settings.Default.DrCycleTime;
            comboBox1.Text = Properties.Settings.Default.DrPower;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select DR power");
                return;
            }
            int d;
            bool b = int.TryParse(textBox1.Text, out d);
            if (b == false)
            {
                MessageBox.Show("Please select DR cycle time");
                return;
            }
            Properties.Settings.Default.DrCycleTime = textBox1.Text;
            Properties.Settings.Default.DrPower = comboBox1.Text;
            Properties.Settings.Default.Save();

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
