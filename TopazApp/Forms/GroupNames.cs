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
    public partial class GroupNames : Form
    {

        List<string> l;

        List<string> m_toAdd = new List<string>();
        string m_groupName = "none";
        public GroupNames()
        {
            InitializeComponent();

            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            l = MySQLConnector.GetGroupNames();

            foreach (string s in l)
            {
                listBox1.Items.Add(s);
            }

        
        }

        public string GroupName
        {
            get
            {
                return m_groupName;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Contains(textBox1.Text))
            {
                MessageBox.Show("Already exists");
                return;
            }
            listBox1.Items.Add(textBox1.Text);

            m_toAdd.Add(textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            MySQLConnector.AddNewGroupNames(m_toAdd);

            if (listBox1.SelectedIndex > -1)
            {
                m_groupName = listBox1.Items[listBox1.SelectedIndex].ToString();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }

            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {


            Close();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
            {
                MySQLConnector.AddNewGroupNames(m_toAdd);
                m_groupName = listBox1.Items[listBox1.SelectedIndex].ToString();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
        }
    }
}
