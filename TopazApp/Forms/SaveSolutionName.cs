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
    public partial class SaveSolutionName : Form
    {
        string m_utensilName;
        string m_solutionName;
        string m_dishname;
        string m_desc;
        float m_drpower;
        ushort m_drCycleTime;
        public SaveSolutionName()
        {
            InitializeComponent();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            List<string> l = MySQLConnector.GetUtensilList();

            foreach (string s in l)
            {
                comboBox1.Items.Add(s);
            }

            try
            {
                List<DISHInfo> list = MySQLConnector.GetAllDishNames();
                foreach (DISHInfo s in list)
                {
                    comboBox2.Items.Add(s.dishName);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void SaveSolutionName_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            if (textBox1.Text == string.Empty)
            {
                MessageBox.Show("Please enter a cooking solution name");
                return;
            }
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select utensil for this solution");
                return;
            }

            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Please select dish name for this solution");
                return;
            }

            if (comboBox3.SelectedIndex == -1)
            {
                MessageBox.Show("Please select dr power for this solution");
                return;
            }
            if (textBox3.Text == string.Empty)
            {
                MessageBox.Show("Please enter DR cycle time for this solution");
                return;
            }

            bool b = ushort.TryParse(textBox3.Text, out m_drCycleTime);
            if (b == false)
            {
                 MessageBox.Show("Please enter valid DR cycle time for this solution");
                return;
            }

            m_solutionName = textBox1.Text;
            m_utensilName = comboBox1.Text;
            m_dishname = comboBox2.Text;
            if (comboBox3.Text == "Same as algo power")
                m_drpower = 0;
            else
                m_drpower = float.Parse(comboBox3.Text);
            m_desc = textBox2.Text;
            Close();
        }

        public string DishName
        {
            get
            {
                return m_dishname;
            }
        }

        public string Utensil
        {
            get
            {
                return m_utensilName;
            }
        }
        public string SolutionName
        {
            get
            {
                return m_solutionName;
            }
        }
        public ushort DRCycleTime
        {
            get
            {
                return m_drCycleTime;
            }
        }
        public float DRPower
        {
            get
            {
                return m_drpower;
            }
        }
        public string Description
        {
            get
            {
                return m_desc;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DishNames d = new DishNames();
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    comboBox2.Items.Clear();
                    List<DISHInfo> list = MySQLConnector.GetAllDishNames();
                    foreach (DISHInfo s in list)
                    {
                        comboBox2.Items.Add(s.dishName);
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
                comboBox2.Text = d.DishName;
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ManagedUtensilsForm f = new ManagedUtensilsForm();
            f.ShowDialog();

            comboBox1.Items.Clear();
            List<string> l = MySQLConnector.GetUtensilList();
            foreach (string s in l)
            {
                comboBox1.Items.Add(s);
            }


        }
    }
}
