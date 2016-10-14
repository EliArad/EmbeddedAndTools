using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopazApp.TOPNAT
{
    public partial class LoadTestForm : Form
    {
        List<TestInfo> l;
        int m_testid = -1;

        List<User> m_userlist;
        TestInfo m_testInfo;
        string m_orderBy = string.Empty;
        ASCD m_acdc = ASCD.DESC;

        public LoadTestForm()
        {
            InitializeComponent();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            comboBox3.SelectedIndex = 0;
            try
            {
                radioButton2.Checked = true;
                ShowTestList(string.Empty, string.Empty);
                m_userlist = MySQLConnector.GetAllUsers();
                comboBox2.Items.Add("All users");
                foreach (User u in m_userlist)
                {
                    string str = string.Format("{0} {1}", u.firstName , u.lastName);
                    comboBox2.Items.Add(str);
                }
                comboBox2.SelectedIndex = 0;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

        }
        void ShowTestList(string firstName , string lastName)
        {
            l = MySQLConnector.GetTestList(firstName, lastName, m_orderBy, m_acdc);
            comboBox1.Items.Clear();
            string str;
            foreach (TestInfo d in l)
            {
                str = string.Format("({0}) - {1} - {2}", d.id, d.testname, d.StartDated);
                comboBox1.Items.Add(str);
            }
        }
        public TestInfo testInfo
        {
            get
            {
                return m_testInfo;
            }
        }
        public int TestID
        {
            get
            {
                return m_testid;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select test");
                return;
            }
            m_testid = l[comboBox1.SelectedIndex].id;
            m_testInfo = l[comboBox1.SelectedIndex];
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
                return;
            textBox1.Text = l[comboBox1.SelectedIndex].remark;
            m_testInfo = l[comboBox1.SelectedIndex];


            label3.Text = m_testInfo.utensil_name;
            label4.Text = m_testInfo.drpower.ToString();
            label7.Text = m_testInfo.dish_name;
            label11.Text = m_testInfo.fres.PassFail.ToString();
            textBox3.Text = m_testInfo.fres.FinalDescription;
            label13.Text = m_testInfo.StartDated.ToString();
            TimeSpan tdiff = m_testInfo.StopDated - m_testInfo.StartDated;
            label14.Text = tdiff.ToString();
            label16.Text = m_testInfo.fres.totalkj.ToString();
            label18.Text = m_testInfo.fres.avgfordb.ToString();

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowListWithUserCeriteria();
        }
        void ShowListWithUserCeriteria()
        {
            try
            {
                if (comboBox2.SelectedIndex == -1)
                    return;
                if (comboBox2.Text != "All users")
                {
                    string name = comboBox2.Text;
                    string[] words = name.Split(new Char[] { ' ' });

                    ShowTestList(words[0], words[1]);
                }
                else
                {
                    ShowTestList(string.Empty, string.Empty);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_orderBy = comboBox3.Text;
            ShowListWithUserCeriteria();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            m_acdc = ASCD.DESC;
            ShowListWithUserCeriteria();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            m_acdc = ASCD.ASC;
            ShowListWithUserCeriteria();
        }
    }
}
