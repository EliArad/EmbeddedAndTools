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

namespace TopazDemoApp.AlgoParamsForms
{
    public partial class AlgoTopPercentageForm : Form
    {
        string m_lastTime;
        Form1.TopPercentageParams4Gui m_topParams;
        int m_drCycleTime;

        public AlgoTopPercentageForm(Object par, int drCycleTime)
        {
            InitializeComponent();
            m_topParams = (Form1.TopPercentageParams4Gui)par;
            m_drCycleTime = drCycleTime;
            
            radioButton4.Checked = m_topParams.agc;
            checkBox1.Checked = m_topParams.equaltime == 1 ? true : false;
            checkBox2.Checked = m_topParams.equalEnergy;
            textBox1.Text = m_topParams.powertime.ToString();
            textBox2.Text = m_topParams.toppercent.ToString();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            m_lastTime = textBox1.Text;

            bool b = ushort.TryParse(textBox2.Text, out m_topParams.toppercent);
            if (b == false)
            {
                textBox2.Text = "25";
            }
            else if (m_topParams.toppercent == 0)
            {
                textBox2.Text = "25";
            }

        }

        public Object GetParams()
        {
            return m_topParams;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                bool b = ushort.TryParse(textBox2.Text, out m_topParams.toppercent);
                if (b == false)
                {
                    MessageBox.Show("Top Percentage is zero");
                    return;
                }

                m_topParams.agc = radioButton4.Checked;
                m_topParams.equaltime  = (ushort)(checkBox1.Checked == true? 1: 0);
                m_topParams.equalEnergy = checkBox2.Checked;
                m_topParams.powertime = ushort.Parse(textBox1.Text);
                          
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBox1.Checked == true)
                {
                    m_lastTime = textBox1.Text;
                    checkBox2.Checked = false;
                    textBox1.Text = ((m_drCycleTime * 1000) / (int.Parse(textBox2.Text))).ToString();
                }
                else
                {
                    textBox1.Text = m_lastTime;
                }
            }
            catch (Exception err)
            {

            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                int r;
                bool b = int.TryParse(textBox2.Text, out r);
                if (b)
                {
                    if (r == 0)
                        return;
                    textBox1.Text = (10000 / r).ToString();
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                checkBox1.Checked = false;
                textBox1.Enabled = false;
            }  
        }
    }
}
