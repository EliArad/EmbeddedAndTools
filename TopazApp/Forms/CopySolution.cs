using Microsoft.Win32;
using RegistryClassApi;
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
    public partial class CopySolution : Form
    {
        List<User> list;
        int m_userId;
        int m_selectedUserId;
        DISH_SOLUTION m_copySolution;

        Dictionary<string, DISH_SOLUTION> listSolutions;

        string m_ovenGuid;
        public CopySolution(int myuserId, string ovenGuid)
        {
            InitializeComponent();
            m_userId = myuserId;
            m_ovenGuid = ovenGuid;
             
            try
            {
                list = MySQLConnector.GetAllUsers();
                foreach( User u in list)
                {
                    comboBox2.Items.Add(u.firstName + " " + u.lastName);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select solution to copy");
                return;
            }
            try
            {
                List<SolutionAlgo> solutionAlgos = MySQLConnector.GetSolutionAlgorithems(m_copySolution.id);

                clsRegistry reg = new clsRegistry();
                string ovenGuid = reg.GetStringValue(Registry.LocalMachine, "SOFTWARE\\Goji solutions\\Field", "Guid");
                if (reg.strRegError == null)
                {                  
                    throw (new SystemException("No oven guid"));
                }

                MySQLConnector.SaveSolution(textBox1.Text,
                                            m_copySolution.description,
                                            m_copySolution.utensilName,
                                            m_copySolution.dishName,
                                            m_userId,
                                            solutionAlgos, m_copySolution.totalTime,
                                            m_copySolution.TotalKj,
                                            m_copySolution.drpower,
                                            ovenGuid, 
                                            m_copySolution.drCycleTime, m_copySolution.groupid);

                MessageBox.Show("Copied ok");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = !checkBox1.Checked;
            if (checkBox1.Checked == true)
            {
                m_selectedUserId = m_userId;
            }
            else
            {
                if (comboBox2.SelectedIndex == -1)
                    return;

                m_selectedUserId = list[comboBox2.SelectedIndex].ID;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == -1)
                return;

            m_selectedUserId = list[comboBox2.SelectedIndex].ID;

            string guid = checkBox2.Checked == true ? m_ovenGuid : string.Empty;
            listSolutions = MySQLConnector.getSolutionsList(m_selectedUserId, true, "ALL", guid);

            comboBox1.Items.Clear();
            foreach (var item in listSolutions.Keys)
            {
                comboBox1.Items.Add(item);
            }

            comboBox3.Items.Clear();
            foreach (var item in listSolutions.Values)
            {
                if (item.dishName != null && item.dishName != "")
                    comboBox3.Items.Add(item.dishName);
            }

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
                return;

            m_copySolution = listSolutions[comboBox1.Text];

            textBox1.Text = comboBox1.Text;
        }
    }
}
