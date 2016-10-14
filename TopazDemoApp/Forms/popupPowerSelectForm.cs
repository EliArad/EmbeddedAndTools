using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopazDemoApp.Forms
{
    public partial class popupPowerSelectForm : Form
    {
        double m_power;
        public popupPowerSelectForm()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
        }

        public double Power
        {
            get
            {
                return m_power;
            }
        }
        private void popupPowerSelectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                e.Cancel = true;
                return;
            }
            m_power = double.Parse(comboBox1.Text);
        }
    }
}
