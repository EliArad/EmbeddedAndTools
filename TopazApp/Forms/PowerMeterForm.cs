using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PowerMeterApp
{

    public partial class PowerMeterForm : Form
    {
        public struct PowerMeterSelect
        {
            public string sensorName;
            public int ampId;
        }

        static int maxPowerMeters = 2;
        string m_str = string.Empty;
        PowerMeterSelect[] pm = new PowerMeterSelect[maxPowerMeters]; 
        public PowerMeterForm()
        {
            InitializeComponent();
            for (int i = 0; i < maxPowerMeters; i++)
            {
                pm[i].ampId = -1;
            }
        }
        public void Add(string SensorName)
        {
            comboBox1.Items.Add(SensorName);
            comboBox2.Items.Add(SensorName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int x = comboBox1.SelectedIndex;
            if (x == -1)
            {
                MessageBox.Show("Select sensor name");
                return;
            }
            pm[0].ampId = 0;
            pm[0].sensorName = comboBox1.Text;

            pm[1].ampId = 1;
            pm[1].sensorName = comboBox2.Text;

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
        public PowerMeterSelect [] getSelection()
        {
            return pm;
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
