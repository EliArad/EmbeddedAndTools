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
    public partial class WeightlossForm : Form
    {
        bool m_close = false;
        bool m_before;
        int m_testId;
        public WeightlossForm(int testId, bool before)
        {
            InitializeComponent();
            m_before = before;
            m_testId = testId;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty)
            {
                MessageBox.Show("Please enter whight in grams");
                return;
            }
            m_close = true;

            float d;
            bool b = float.TryParse(textBox1.Text , out d);
            if (b == false)
            {
                MessageBox.Show("Please enter a coorect whight in grams");
                return;
            }
            try
            {
                MySQLConnector.UpdateWhightLoss(m_testId, d, m_before);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

            Close();
        }

        private void WeightlossForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_close == false)
            {
                e.Cancel = true;
                return;
            }
        }
    }
}
