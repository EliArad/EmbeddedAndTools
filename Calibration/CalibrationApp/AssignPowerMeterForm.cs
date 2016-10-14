using MeasurementsToolsClassLib;
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
    public partial class AssignPowerMeterForm : Form
    {

        static int maxPowerMeters = 2;
        PowerMeterSelect[] pm = new PowerMeterSelect[maxPowerMeters]; 
        public struct PowerMeterSelect
        {
            public string sensorName;
        }
        List<string> m_sensorsName = new List<string>();
        NRP_Z211PowerMeter[] m_powerMeter = { null, null };
        PowerMeterSelect[] pmSelect = new PowerMeterSelect[2];

        public AssignPowerMeterForm()
        {
            InitializeComponent();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;


            try
            {
                int count = FillRodeSwartzPowerMeterList();
                if (count == 0)
                {
                    MessageBox.Show("No power meter found");
                    return;
                }
                comboBox1.Items.Add("None");
                comboBox2.Items.Add("None");
                for (int i = 0 ; i < m_sensorsName.Count; i++)
                {
                    comboBox1.Items.Add(m_sensorsName[i]);
                    comboBox2.Items.Add(m_sensorsName[i]);
                }                   
                LoadAssigments();
                if (pmSelect[0].sensorName != null && pmSelect[0].sensorName != "")
                {
                    label3.Text = pmSelect[0].sensorName;
                    comboBox1.Text = pmSelect[0].sensorName;
                }
                if (pmSelect[1].sensorName != null && pmSelect[1].sensorName != "")
                {
                    label4.Text = pmSelect[1].sensorName;
                    comboBox2.Text = pmSelect[1].sensorName;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        void LoadAssigments()
        {

            pmSelect[0].sensorName = Properties.Settings.Default.PowerMeter_forward;
            pmSelect[1].sensorName = Properties.Settings.Default.PowerMeter_reflected;

        }
        void SaveAssigments()
        {
            Properties.Settings.Default.PowerMeter_forward = pmSelect[0].sensorName;
            Properties.Settings.Default.PowerMeter_reflected = pmSelect[1].sensorName;
            Properties.Settings.Default.Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pmSelect[0].sensorName = comboBox1.Text;
            pmSelect[1].sensorName = comboBox2.Text;

            SaveAssigments();

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        } 
        private int FillRodeSwartzPowerMeterList()
        {
            try
            {
                int c = NRP_Z211PowerMeter.GetSensorCount();
                if (c == 0)
                {
                    return c;
                }

                string SensorType = string.Empty;
                string SensorName = string.Empty;
                string SensorSerial = string.Empty;
                for (int i = 1; i < (c + 1); i++)
                {
                    NRP_Z211PowerMeter.GetSensorInfo(i,
                                         out SensorType,
                                         out SensorName,
                                         out SensorSerial);

                    m_sensorsName.Add(SensorName);
                }
                return c;
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                int count = FillRodeSwartzPowerMeterList();
                if (count == 0)
                {
                    MessageBox.Show("No power meter found");
                    return;
                }
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();
                comboBox1.Items.Add("None");
                comboBox2.Items.Add("None");
                for (int i = 0; i < m_sensorsName.Count; i++)
                {
                    comboBox1.Items.Add(m_sensorsName[i]);
                    comboBox2.Items.Add(m_sensorsName[i]);
                }                
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label1.Text = comboBox1.Text;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            label2.Text = comboBox2.Text;
        }

    }
}
