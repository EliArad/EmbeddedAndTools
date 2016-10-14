using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopazCalibrationApp
{
    public partial class LinearPowerCalSetting : Form
    {

        public LinearPowerCalSetting()
        {
            InitializeComponent();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            textBox1.Text = Properties.Settings.Default.DDSStart;
            textBox2.Text = Properties.Settings.Default.DDSStop;
            textBox3.Text = Properties.Settings.Default.DDSStep;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool b;
            int v;
            b = int.TryParse(textBox1.Text, out v);
            if (b == false)
            {
                MessageBox.Show("Error converting DDS Start");
                return;
            }

            b = int.TryParse(textBox2.Text, out v);
            if (b == false)
            {
                MessageBox.Show("Error converting DDS Stop");
                return;
            }
            b = int.TryParse(textBox3.Text, out v);
            if (b == false)
            {
                MessageBox.Show("Error converting DDS Step");
                return;
            }

            Properties.Settings.Default.DDSStart = textBox1.Text;
            Properties.Settings.Default.DDSStop = textBox2.Text;
            Properties.Settings.Default.DDSStep = textBox3.Text;
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
