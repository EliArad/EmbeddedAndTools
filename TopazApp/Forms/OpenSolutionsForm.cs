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
    public partial class OpenSolutionsForm : Form
    {
        int m_userId;
        Dictionary<string, DISH_SOLUTION> list;
        string m_ovenGuid;
        public OpenSolutionsForm(int userId, string ovenGuid)
        {
            InitializeComponent();
            m_ovenGuid = ovenGuid;
            m_userId = userId;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            checkBox1.Checked = Properties.Settings.Default.ShowSolutionOfThisOven;

            this.Text = "Open solutions - showing your solutions only";

            try
            {
                LoadSolutions(userId);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

            try
            {
                List<DISHInfo> list = MySQLConnector.GetAllDishNames();
                comboBox2.Items.Add("ALL");
                foreach (DISHInfo s in list)
                {
                    comboBox2.Items.Add(s.dishName);
                }
                comboBox2.SelectedIndex = 0;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

        }

        void LoadSolutions(int userId)
        {
            try
            {
                comboBox1.Items.Clear();
                string guid = checkBox1.Checked == true ? m_ovenGuid : string.Empty;
                list = MySQLConnector.getSolutionsList(userId, true, "ALL", guid);

                foreach (KeyValuePair<string, DISH_SOLUTION> entry in list)
                {
                    comboBox1.Items.Add(entry.Key);
                }               
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select dish solution from the list");
                return;
            }
            Properties.Settings.Default.ShowSolutionOfThisOven = checkBox1.Checked;
            Properties.Settings.Default.Save();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();            
        }
        public string SolutionName
        {
            get
            {
                if (comboBox1.SelectedIndex == -1)
                    return string.Empty;
                return comboBox1.Text;
            }
        }
        public int SolutionId
        {
            get
            {
                return list[comboBox1.Text].id;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                LoadSolutions(m_userId);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
                return;
            try
            {
                DISH_SOLUTION d = MySQLConnector.GetSolutionInfo(comboBox1.Text, 
                                                                 comboBox2.Text, 
                                                                 m_userId);
                textBox2.Text = d.description;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not implemented yet , if you want to save description changes let Eli Arad know");
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                comboBox1.Items.Clear();
                string guid = checkBox1.Checked == true ? m_ovenGuid : string.Empty;
                list = MySQLConnector.getSolutionsList(m_userId, true, comboBox2.Text, guid);

                foreach (KeyValuePair<string, DISH_SOLUTION> entry in list)
                {
                    comboBox1.Items.Add(entry.Key);
                }
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenSolutionAdvancedSearchForm f = new OpenSolutionAdvancedSearchForm();
            f.ShowDialog();
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            try
            {
                comboBox1.Items.Clear();
                string guid = checkBox1.Checked == true ? m_ovenGuid : string.Empty;
                list = MySQLConnector.getSolutionsList(m_userId, true, comboBox2.Text, guid);

                foreach (KeyValuePair<string, DISH_SOLUTION> entry in list)
                {
                    comboBox1.Items.Add(entry.Key);
                }
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
    }
}
