using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopazApp.Forms
{
    public partial class PassFailForm : Form
    {
        int m_passfail;
        string m_desc;
        bool allow = false;
        public PassFailForm(int score_end)
        {
            InitializeComponent();
            comboBox1.Items.Clear();
            for (int i = 1; i <= score_end; i++)
            {
                comboBox1.Items.Add(i);
            }
            comboBox1.SelectedIndex = 4;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        public string Description
        {
            get
            {
                return m_desc;
            }
        }
        public int PassFail
        {
            get
            {
                return m_passfail;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                if (textBox1.Text == string.Empty)
                {
                    MessageBox.Show("Please enter a cooking description");
                    return;
                }
                m_desc = textBox1.Text;
                if (comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select pass fail value from 1-10");
                    return;
                }
                m_passfail = int.Parse(comboBox1.Text);

                allow = true;
                Close();
            }
        }

        private void PassFailForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            lock (this)
            {
                if (allow == false)
                    e.Cancel = true;
            }
        }
    }
}
