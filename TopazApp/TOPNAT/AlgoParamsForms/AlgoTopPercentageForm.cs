using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopazApp.AlgoParamsForms
{
    public partial class AlgoTopPercentageForm1 : Form
    {
        string freqtableParams = string.Empty;
        AlgoTopPercentageParams? m_params;
        string m_lastTime;
        public AlgoTopPercentageForm1(AlgoTopPercentageParams? d)
        {
            InitializeComponent();
        
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            if (d == null)
            {
                m_params = new AlgoTopPercentageParams();
                m_lastTime = textBox1.Text;
            }
            else
            {
                m_params = d.Value;
                textBox1.Text = m_params.Value.power_time_mili.ToString();
                m_lastTime = textBox1.Text;
                textBox2.Text = m_params.Value.toppercent.ToString();
                radioButton4.Checked = m_params.Value.agc;
                radioButton5.Checked = !m_params.Value.agc;
                checkBox1.Checked = m_params.Value.equaldrtime;
                freqtableParams = m_params.Value.freqtablefilename;
                if (textBox1.Text == "400")
                {
                    checkBox1.Checked = true;
                }
            }
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

                AlgoTopPercentageParams x = new AlgoTopPercentageParams();
                x.freqtablefilename = freqtableParams;
                x.power_time_mili = int.Parse(textBox1.Text);
                x.agc = radioButton4.Checked;
                x.toppercent = int.Parse(textBox2.Text);
                x.equaldrtime = checkBox1.Checked;
                if (x.toppercent == 0 || x.toppercent > 100)
                {
                    MessageBox.Show("Invalid value for top percentage 1 - 100 is allowed");
                    return;
                }
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
        public AlgoTopPercentageParams GetParams()
        {
            return m_params.Value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBox1.Checked)
                {
                    m_lastTime = textBox1.Text;
                    textBox1.Text = (10000 / int.Parse(textBox2.Text)).ToString();
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
                    textBox1.Text = (10000 / r).ToString();
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            freqtableform1 f = new freqtableform1(m_params.Value.sid, m_params.Value.RowIndex, "Top Percentage");
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                freqtableParams = f.GetFileName();
            }
        }

        private void AlgoTopPercentageForm1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
