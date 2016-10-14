using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopazApi;
using TopazDemoApp.AlgoParamsForms;

namespace TopazDemoApp.Forms
{
    public partial class EqualEnergyAlgoForm : Form
    {
       
        string freqtableParams = string.Empty;
        Form1.EqualEnergyParams4Gui epar;
        public EqualEnergyAlgoForm(Object par, int drCycleTime)
        {
            InitializeComponent();

            epar = (Form1.EqualEnergyParams4Gui)par;
            radioButton4.Checked = epar.agc;
            textBox2.Text = epar.acckj.ToString();           
            radioButton5.Checked =  !epar.agc;
            checkBox1.Checked = epar.singlerepetition;
            switch (epar.mode)
            {
                case 0:
                    numericUpDown1.Value = (decimal)epar.highvalue;
                    radioButton1.Checked = true;
                    numericUpDown1.Enabled = true;

                    numericUpDown2.Enabled = false;
                    numericUpDown3.Enabled = false;
                    numericUpDown4.Enabled = false;
                    break;
                case 1:
                    numericUpDown2.Value = (decimal)epar.lowvalue;
                    radioButton2.Checked = true;
                    numericUpDown2.Enabled = true;

                    numericUpDown1.Enabled = false;
                    numericUpDown3.Enabled = false;
                    numericUpDown4.Enabled = false;
                    break;
                case 2:
                    numericUpDown3.Value = (decimal)epar.lowvalue;
                    numericUpDown4.Value = (decimal)epar.highvalue;
                    radioButton3.Checked = true;

                    numericUpDown3.Enabled = true;
                    numericUpDown4.Enabled = true;

                    numericUpDown1.Enabled = false;
                    numericUpDown2.Enabled = false;
                    break;
                case 3:
                    textBox1.Text = epar.toppercentage.ToString();
                    radioButton6.Checked = true;
                    break;
            }
          
            
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {

            try
            {

                if (radioButton4.Checked == false && radioButton5.Checked == false)
                {
                    MessageBox.Show("Please select the type of AGC mode");
                    return;
                }
                float acckj = 0;
                if (checkBox1.Checked == false)
                {                
                    bool b = float.TryParse(textBox2.Text, out acckj);
                    if (b == false)
                    {
                        MessageBox.Show("Accumolated KJ is not determine yet");
                        return;
                    }
                    if (acckj == 0)
                    {
                        MessageBox.Show("Accumolated KJ cannot be zero");
                        return;
                    }
                }

                if (radioButton1.Checked == true)
                {
                    epar.highvalue = float.Parse(numericUpDown1.Value.ToString());
                    epar.mode = 0;
                }
                if (radioButton2.Checked == true)
                {
                    epar.lowvalue = float.Parse(numericUpDown2.Value.ToString());
                    epar.mode = 1;
                }
                if (radioButton3.Checked == true)
                {
                    epar.lowvalue = float.Parse(numericUpDown3.Value.ToString());
                    epar.highvalue = float.Parse(numericUpDown4.Value.ToString());
                    epar.mode = 2;
                }
                if (radioButton6.Checked == true)
                {
                    epar.toppercentage = ushort.Parse(textBox1.Text);
                    epar.mode = 3;
                }
                epar.singlerepetition = checkBox1.Checked;
                epar.acckj =  acckj;
                epar.agc = radioButton4.Checked;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }


            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        public Object GetParams()
        {
            return epar;
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
             
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = !checkBox1.Checked;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
    }
}
