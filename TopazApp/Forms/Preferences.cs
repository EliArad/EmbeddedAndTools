using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopazApp.Forms
{
    public partial class Preferences : Form
    {
      
        int m_userId;
        DISH_SOLUTION m_curSolution;
        public Preferences(DISH_SOLUTION cursolution, int userId)
        {
            InitializeComponent();
            label2.Text = string.Empty;
            m_curSolution = cursolution;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            m_userId = userId;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            List<string> l = MySQLConnector.GetUtensilList();

            foreach (string s in l)
            {
                comboBox1.Items.Add(s);
            }
            label2.Text = m_curSolution.name;


            try
            {
                comboBox3.Items.Clear();
                List<DISHInfo> list = MySQLConnector.GetAllDishNames();
                foreach (DISHInfo s in list)
                {
                    comboBox3.Items.Add(s.dishName);
                }
                comboBox3.Text = m_curSolution.dishName;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

            if (m_curSolution.drpower == 0)
                comboBox2.SelectedIndex = 0;
            else 
                comboBox2.Text = m_curSolution.drpower.ToString();

            textBox3.Text = m_curSolution.drCycleTime.ToString();

            string gn = MySQLConnector.GetGroupName(m_curSolution.groupid);
            linkLabel3.Text = "Group name:" + gn;
            
        }
        public T DeepCopy<T>(T obj)
        {
            BinaryFormatter s = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                s.Serialize(ms, obj);
                ms.Position = 0;
                T t = (T)s.Deserialize(ms);

                return t;
            }
        }
        public DISH_SOLUTION GetDishSolution()
        {
            return m_curSolution;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ushort cycle;
                bool b = ushort.TryParse(textBox3.Text, out cycle);
                if (b == false)
                {
                    MessageBox.Show("DR Cycle time is invalid");
                    return;
                }
                
                DISH_SOLUTION d = new DISH_SOLUTION();

                m_curSolution.dishName = comboBox3.Text;

                d.id             =      m_curSolution.id       ;                  
                d.name           =      m_curSolution.name      ;           
                d.createdBy       =     m_curSolution.createdBy  ;      
                d.Dated          =      m_curSolution.Dated       ;
                d.description    =      m_curSolution.description  ;     
                d.updatedBy     =       m_curSolution.updatedBy  ;
                d.updated       =       m_curSolution.updated     ; 
                d.utensilName    =      m_curSolution.utensilName;
                d.dishName       =      m_curSolution.dishName   ;
                d.firstname      =      m_curSolution.firstname   ;
                d.lastname       =      m_curSolution.lastname     ;
                d.totalTime      =      m_curSolution.totalTime     ;  
                d.TotalKj        =      m_curSolution.TotalKj    ;
                d.drpower = m_curSolution.drpower;

                d.drCycleTime = cycle;
                        
                //d = DeepCopy<DISH_SOLUTION>(m_curSolution);
                d.utensilName = comboBox1.Text;
                if (comboBox2.Text == "Same as algo power")
                {
                    d.drpower = 0;
                }
                else
                {
                    d.drpower = float.Parse(comboBox2.Text);
                }
                d.description = textBox2.Text;
                m_curSolution.drCycleTime = d.drCycleTime;
                m_curSolution.description = d.description;
                m_curSolution.drpower = d.drpower;
                m_curSolution.utensilName = d.utensilName;
                MySQLConnector.UpdateSolutionInfo(d, m_userId);

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Preferences_Load(object sender, EventArgs e)
        {
          
            try
            {
                //DISH_SOLUTION d = MySQLConnector.GetSolutionInfo(m_curSolution.name, "ALL", m_userId);
                textBox2.Text = m_curSolution.description;
                label5.Text = m_curSolution.firstname + " " + m_curSolution.lastname;
                comboBox1.Text = m_curSolution.utensilName;
                label6.Text = m_curSolution.Dated;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                Close();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                button1.Enabled = true;
            } 
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != string.Empty)
            {
                button1.Enabled = true;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ManagedUtensilsForm f = new ManagedUtensilsForm();
            f.ShowDialog();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DishNames d = new DishNames();
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    comboBox3.Items.Clear();
                    List<DISHInfo> list = MySQLConnector.GetAllDishNames();
                    foreach (DISHInfo s in list)
                    {
                        comboBox3.Items.Add(s.dishName);
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
                comboBox3.Text = d.DishName;
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GroupNames g = new GroupNames();
            if (g.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                linkLabel3.Text = "Group name:" + g.GroupName;
                m_curSolution.groupName = g.GroupName;
                m_curSolution.groupid = MySQLConnector.GetGroupId(g.GroupName);
            }
        }
    }
}
