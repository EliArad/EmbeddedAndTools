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
    public partial class ManagedUtensilsForm : Form
    {
        public ManagedUtensilsForm()
        {
            InitializeComponent();

            List<string> l = MySQLConnector.GetUtensilList();
            foreach (string s in l)
            {
                listBox1.Items.Add(s);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                return;

            try
            {
                string name = listBox1.Items[listBox1.SelectedIndex].ToString();
                MySQLConnector.DeleteUtensilName(name);
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MySQLConnector.AddNewUtensil(textBox1.Text);
                listBox1.Items.Add(textBox1.Text);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}
