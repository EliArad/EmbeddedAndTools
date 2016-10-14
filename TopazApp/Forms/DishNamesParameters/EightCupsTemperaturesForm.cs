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
    public partial class EightCupsTemperaturesForm : Form
    {
        bool m_before;
        int m_testid;
        public EightCupsTemperaturesForm(int testid, bool before)
        {
            InitializeComponent();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            m_before = before;
            m_testid = testid;

            if (before == false)
            {
                label6.Text = "End temperatures";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                float[] temp = {

                                float.Parse(textBox1.Text),
                                float.Parse(textBox5.Text),
                                float.Parse(textBox2.Text),
                                float.Parse(textBox4.Text),
                                float.Parse(textBox7.Text),
                                float.Parse(textBox8.Text),
                                float.Parse(textBox6.Text),
                                float.Parse(textBox3.Text)
                            };

                MySQLConnector.SaveEightCupTemperatures(m_testid, temp, m_before);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void FiveCupsTemperaturesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == System.Windows.Forms.DialogResult.Cancel)
                e.Cancel = true;

        }
    }
}
