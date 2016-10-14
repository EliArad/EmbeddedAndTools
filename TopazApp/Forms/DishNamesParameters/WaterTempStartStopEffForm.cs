using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopazApp.Forms.DishNamesParameters
{
    public partial class WaterTempStartStopEffForm : Form
    {
        int m_testid;
        bool m_start;
        bool letclose = false;
        public WaterTempStartStopEffForm(bool start, int testid)
        {
            InitializeComponent();
            m_testid = testid;
            m_start = start;
            groupBox1.Enabled = start;
            groupBox2.Enabled = !start;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                if (m_start == true)
                {
                    MySQLConnector.SaveWaterTemperatureWeightParams(m_testid,
                                                                    float.Parse(textBox1.Text),
                                                                    float.Parse(textBox3.Text));

                }
                else
                {
                    float efficiency = 0;
                    MySQLConnector.UpdateWaterTemperatureWeightParams(m_testid,
                                                                      float.Parse(textBox2.Text),
                                                                      efficiency);
                                                                      

                }
                letclose = true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void WaterTempStartStopEffForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (letclose == false)
                e.Cancel = true;
        }
    }
}
