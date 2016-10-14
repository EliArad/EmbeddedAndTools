using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopazApp.AlgoParamsForms;

namespace TopazApp.Forms
{
    public partial class DRThresholdAlgoForm : Form
    {
        AlgoThresholParams ? m_params;

        string freqtableParams = string.Empty;
        public DRThresholdAlgoForm(AlgoThresholParams ? d)
        {
            InitializeComponent();
            if (d == null)
            {
                m_params = new AlgoThresholParams();
                radioButton4.Checked = true;
                checkBox1.Checked = true;
            }
            else
            {
                m_params = d.Value;
                textBox1.Text = m_params.Value.power_time_mili.ToString();
                
                radioButton4.Checked = m_params.Value.agc;
                radioButton5.Checked = !m_params.Value.agc;
                checkBox1.Checked = m_params.Value.equaldrtime;
                freqtableParams = m_params.Value.freqtablefilename;
                checkBox2.Checked = m_params.Value.substractEmptyCavity;
                switch (m_params.Value.mode)
                {
                    case 0:
                        numericUpDown1.Value = (decimal)m_params.Value.highvalue;
                        radioButton1.Checked = true;
                        numericUpDown1.Enabled = true;

                        numericUpDown2.Enabled = false;
                        numericUpDown3.Enabled = false;
                        numericUpDown4.Enabled = false;

                        break;
                    case 1:
                        numericUpDown2.Value = (decimal)m_params.Value.lowvalue;
                        radioButton2.Checked = true;
                        numericUpDown2.Enabled = true;

                        numericUpDown1.Enabled = false;
                        numericUpDown3.Enabled = false;
                        numericUpDown4.Enabled = false;
                        break;
                    case 2:
                        numericUpDown3.Value = (decimal)m_params.Value.lowvalue;
                        numericUpDown4.Value = (decimal)m_params.Value.highvalue;
                        radioButton3.Checked = true;

                        numericUpDown3.Enabled = true;
                        numericUpDown4.Enabled = true;

                        numericUpDown1.Enabled = false;
                        numericUpDown2.Enabled = false;
                        break;
                }
            }
            
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        public AlgoThresholParams GetParams()
        {
            return m_params.Value;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                AlgoThresholParams x = new AlgoThresholParams();
                x.freqtablefilename = freqtableParams;
                if (radioButton4.Checked == false && radioButton5.Checked == false)
                {
                    MessageBox.Show("Please select the type of AGC mode");
                    return;
                }

                if (radioButton1.Checked == true)
                {
                    x.highvalue = float.Parse(numericUpDown1.Value.ToString());
                    x.mode = 0;
                }
                if (radioButton2.Checked == true)
                {
                    x.lowvalue = float.Parse(numericUpDown2.Value.ToString());
                    x.mode = 1;
                }
                if (radioButton3.Checked == true)
                {
                    x.lowvalue = float.Parse(numericUpDown3.Value.ToString());
                    x.highvalue = float.Parse(numericUpDown4.Value.ToString());
                    x.mode = 2;
                }
                x.equaldrtime = checkBox1.Checked;
                if (x.equaldrtime == false)
                    x.power_time_mili = int.Parse(textBox1.Text);
                x.agc = radioButton4.Checked;
                x.substractEmptyCavity = checkBox2.Checked;
                m_params = x;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = true;
            
            numericUpDown2.Enabled = false;
            numericUpDown3.Enabled = false;
            numericUpDown4.Enabled = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown2.Enabled = true;

            numericUpDown1.Enabled = false;
            numericUpDown3.Enabled = false;
            numericUpDown4.Enabled = false;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown3.Enabled = true;
            numericUpDown4.Enabled = true;

            numericUpDown1.Enabled = false;
            numericUpDown2.Enabled = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            label2.Enabled = !checkBox1.Checked;
            textBox1.Enabled = !checkBox1.Checked;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            freqtableform f = new freqtableform(m_params.Value.sid, m_params.Value.RowIndex, "DR Treshold");
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                freqtableParams = f.GetFileName();
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EqualEnergyPerDR d = new EqualEnergyPerDR(m_params.Value.sid, m_params.Value.RowIndex, "DR Treshold");
            d.ShowDialog();
        }

        private void DRThresholdAlgoForm_Load(object sender, EventArgs e)
        {
           
        }
    }
}
