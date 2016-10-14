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
    public partial class CreateNewPAInfoForm : Form
    {
        string m_guid;
        int m_userId;
        public CreateNewPAInfoForm(string guid, int userid)
        {
            InitializeComponent();
            m_guid = guid;
            m_userId = userid;
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            try
            {
                string coupler = string.Empty;
                switch (comboBox1.Text)
                {
                    case "Lior Coupler - 01":
                        coupler = "Lior Coupler - 01";
                    break;
                    case "Green coupler - 99832":
                        coupler = "Green coupler - 99832";
                    break;
                }
                MySQLConnector.AddNewOven(m_guid, textBox2.Text, m_userId, textBox1.Text, 
                                          coupler, checkBox1.Checked, 
                                          textBox3.Text);
                Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "CSV Files (.csv) |*.csv";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox3.Text = openFileDialog1.FileName;
            }
        }
    }
}
