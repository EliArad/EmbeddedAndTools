using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopazDemoApp.AlgoParamsForms;

namespace TopazDemoApp.Forms
{
    public partial class DRThresholdAlgoForm : Form
    {
       

        string freqtableParams = string.Empty;
        Form1.DRThresholdParams4Gui dpar;
        int m_drCycleTime;
        public DRThresholdAlgoForm(Object par, int drCycleTime)
        {
            InitializeComponent();
            dpar = (Form1.DRThresholdParams4Gui)par;

            m_drCycleTime = drCycleTime;
            textBox1.Text = dpar.powertime.ToString();

            radioButton4.Checked = dpar.agc;
            radioButton5.Checked = !dpar.agc;
            checkBox1.Checked = dpar.equaldrtime;
            //freqtableParams = dpar.freqtablefilename;
            switch (dpar.mode)
            {
                case 0:
                    numericUpDown1.Value = (decimal)dpar.highvalue;
                    radioButton1.Checked = true;
                    numericUpDown1.Enabled = true;

                    numericUpDown2.Enabled = false;
                    numericUpDown3.Enabled = false;
                    numericUpDown4.Enabled = false;
                break;
                case 1:
                    numericUpDown2.Value = (decimal)dpar.lowvalue;
                    radioButton2.Checked = true;
                    numericUpDown2.Enabled = true;

                    numericUpDown1.Enabled = false;
                    numericUpDown3.Enabled = false;
                    numericUpDown4.Enabled = false;
                    break;
                case 2:
                    numericUpDown3.Value = (decimal)dpar.lowvalue;
                    numericUpDown4.Value = (decimal)dpar.highvalue;
                    radioButton3.Checked = true;

                    numericUpDown3.Enabled = true;
                    numericUpDown4.Enabled = true;

                    numericUpDown1.Enabled = false;
                    numericUpDown2.Enabled = false;
                    break;
            }
             
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        public Object GetParams()
        {
            return dpar;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                //x.freqtablefilename = freqtableParams;
                if (radioButton4.Checked == false && radioButton5.Checked == false)
                {
                    MessageBox.Show("Please select the type of AGC mode");
                    return;
                }

                if (radioButton1.Checked == true)
                {
                    dpar.highvalue = float.Parse(numericUpDown1.Value.ToString());
                    dpar.mode = 0;
                }
                if (radioButton2.Checked == true)
                {
                    dpar.lowvalue = float.Parse(numericUpDown2.Value.ToString());
                    dpar.mode = 1;
                }
                if (radioButton3.Checked == true)
                {
                    dpar.lowvalue = float.Parse(numericUpDown3.Value.ToString());
                    dpar.highvalue = float.Parse(numericUpDown4.Value.ToString());
                    dpar.mode = 2;
                }
                dpar.equaldrtime = checkBox1.Checked;
                if (dpar.equaldrtime == false)
                    dpar.powertime = ushort.Parse(textBox1.Text);
                dpar.agc = radioButton4.Checked;
               
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
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
        }

        private void DRThresholdAlgoForm_Load(object sender, EventArgs e)
        {
           
        }
    }
}
