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

namespace TopazApp.Forms.DishNamesParameters
{
    public partial class DefrostingMincedBeef3kgForm : Form
    {
        int m_testId;
        bool m_close = false;
        float[] m_temp = new float[7];
        float m_frozenPart;
        float m_cookedPart;
        public DefrostingMincedBeef3kgForm(string dishName, int testId)
        {
            InitializeComponent();
            m_testId = testId;
            Label[] list = { label1, label2, label3, label4, label5, label6, label7 };
            foreach (Label l in list)
            {
                l.Visible = false;
            }

            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            bool found;
            DISHInfo d = MySQLConnector.GetDishInfo(dishName, out found);
            if (found)
            {
                pictureBox1.ImageLocation = DRIVE.Drive + @"TopazPOC\DishPictures\" + d.picture1;
            }
            else
            {
                MessageBox.Show("Dish does not have default picure1");
                m_close = true;
            }
        }

        private void DefrostingMincedBeef3kgForm_Load(object sender, EventArgs e)
        {
            if (m_close == true)
                Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Label[] list = { label1, label2, label3, label4, label5, label6, label7 };
            foreach (Label l in list)
            {
                l.Visible = checkBox1.Checked;
            }
        }

        public float[] getTemperatures()
        {
            return m_temp;
        }
        private void DefrostingMincedBeef3kgForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            TextBox[] t = { txtTemp1, txtTemp2, txtTemp3, txtTemp4, txtTemp5, txtTemp6, txtTemp7, textBox8, textBox9 };
            foreach(TextBox x in t)
            {
                if (x.Text == string.Empty)
                {
                    MessageBox.Show("You did not filled all the infomation");
                    e.Cancel = true;
                    return;
                }
            }

            TextBox[] temp = { txtTemp1, txtTemp2, txtTemp3, txtTemp4, txtTemp5, txtTemp6, txtTemp7 };

            int i = 0;
            bool b = false;
            foreach (TextBox x in temp)
            {
                b = float.TryParse(x.Text, out m_temp[i]);
                if (b == false)
                {
                    MessageBox.Show("Invalid value for temperature: " + (i + 1));
                    e.Cancel = true;
                    return;
                }
                i++;
            }

            b = float.TryParse(textBox8.Text, out m_frozenPart);
            if (b == false)
            {
                MessageBox.Show("Invalid value for frozen part: " + (i + 1));
                e.Cancel = true;
                return;
            }

            b = float.TryParse(textBox9.Text, out m_cookedPart);
            if (b == false)
            {
                MessageBox.Show("Invalid value for cooked part: " + (i + 1));
                e.Cancel = true;
                return;
            }

            try
            {
                MySQLConnector.InsertDefrost_temp8(m_testId, m_temp, m_frozenPart, m_cookedPart, "after");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}
