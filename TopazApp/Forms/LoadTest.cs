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
    public partial class LoadTest : Form
    {
        List<TestInfo> m_testList = null;
        int m_userId;
        Dictionary<string, TestInfo> m_dicList = new Dictionary<string, TestInfo>();
        TestInfo m_testInfo = new TestInfo();
        DISH_SOLUTION m_newSolution = new DISH_SOLUTION();
        List<SolutionAlgo> m_solutionAlgo = new List<SolutionAlgo>();
        int m_showindexof = 0;

        List<User> m_users;
        public LoadTest(int userid)
        {
            InitializeComponent();
            m_userId = userid;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            label4.Text = "";
            label3.Text = "";
            label7.Text = "";
            try
            {
                m_testList = MySQLConnector.GetMyTestListWithResults(userid, 
                                                                     "date", ASCD.DESC, m_showindexof, 20);
                
                foreach (TestInfo t in m_testList)
                {
                    listBox1.Items.Add(t.testname);
                    m_dicList.Add(t.testname, t);
                }

                string userName = "";
                m_users = MySQLConnector.GetAllUsers();
                foreach (User t in m_users)
                {
                    comboBox1.Items.Add(t.firstName + " " + t.lastName);
                    if (t.ID == userid)
                        userName = t.firstName + " " + t.lastName;
                }
                comboBox1.Text = userName;

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

        }

        public DISH_SOLUTION Solution
        {
            get
            {
                return m_newSolution;
            }
        }
        public List<SolutionAlgo>  SolutionAlgo
        {
            get
            {
                return m_solutionAlgo;
            }
        }
        public TestInfo TestInformation
        {
            get
            {
                return m_testInfo;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a test from the list");
                return;
            }

            string testName = listBox1.Items[listBox1.SelectedIndex].ToString();
            
            
            if (m_dicList.ContainsKey(testName) == true)
            {
                m_testInfo = m_dicList[testName];
                m_newSolution = new DISH_SOLUTION();
                m_newSolution.createdBy = m_userId;
                string addedDate = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");
                m_newSolution.Dated = addedDate;
                m_newSolution.createdBy = m_dicList[testName].user_id;
                m_newSolution.description = m_dicList[testName].description;
                m_newSolution.dishName = m_dicList[testName].dish_name;
                m_newSolution.drpower = m_dicList[testName].drpower;
                m_newSolution.utensilName = m_dicList[testName].utensil_name;
                m_newSolution.guid = m_dicList[testName].guid;
                m_newSolution.TotalKj = m_dicList[testName].totalKj;
                m_newSolution.drCycleTime = m_dicList[testName].drCycleTime;
                m_newSolution.totalTime = m_dicList[testName].totalTime;
                textBox2.Text = m_testInfo.description;
            }
            
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string s = listBox1.Items[listBox1.SelectedIndex].ToString();
                if (m_dicList.ContainsKey(s))
                {
                    m_testInfo = m_dicList[s];
                    textBox2.Text = m_testInfo.remark;
                    label3.Text = m_testInfo.utensil_name;
                    label4.Text = m_testInfo.drpower.ToString();
                    label7.Text = m_testInfo.dish_name;
                    label20.Text = m_testInfo.id.ToString();
                    label22.Text = m_testInfo.drCycleTime.ToString();
                    label11.Text = m_testInfo.fres.PassFail.ToString();
                    textBox3.Text = m_testInfo.fres.FinalDescription;
                    label13.Text = m_testInfo.StartDated.ToString();
                    TimeSpan tdiff = m_testInfo.StopDated - m_testInfo.StartDated;
                    label14.Text = tdiff.ToString();
                    label16.Text = m_testInfo.fres.totalkj.ToString();
                    label18.Text = m_testInfo.fres.avgfordb.ToString();
                }
            }
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                m_showindexof = 0;
                ShowTests();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (m_showindexof>= 20)
                    m_showindexof -= 20;
                ShowTests();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        void ShowTests()
        {
            try
            {

                listBox1.Items.Clear();
                m_dicList.Clear();
                m_testList = MySQLConnector.GetMyTestListWithResults(m_userId,
                                                                    "date",
                                                                    ASCD.DESC, 
                                                                    m_showindexof, 
                                                                    20);

                foreach (TestInfo t in m_testList)
                {
                    listBox1.Items.Add(t.testname);
                    m_dicList.Add(t.testname, t);
                }
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        void ShowTestsByNearTestName(string names)
        {
            try
            {

                listBox1.Items.Clear();
                m_dicList.Clear();
                m_testList = MySQLConnector.GetTestListByNearTestName(m_userId,
                                                                        "date",
                                                                        ASCD.DESC,
                                                                        names,
                                                                        m_showindexof, 
                                                                        20);

                foreach (TestInfo t in m_testList)
                {
                    listBox1.Items.Add(t.testname);
                    m_dicList.Add(t.testname, t);
                }
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                m_showindexof+=20;
                ShowTests();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
       

            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a test from the list");
                return;
            }

            string testName = listBox1.Items[listBox1.SelectedIndex].ToString();
            

            if (m_dicList.ContainsKey(testName) == true)
            {
                m_testInfo = m_dicList[testName];
                m_newSolution = new DISH_SOLUTION();
                m_newSolution.createdBy = m_userId;
                string addedDate = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");
                m_newSolution.Dated = addedDate;
                m_newSolution.TotalKj = m_dicList[testName].totalKj;
                m_newSolution.createdBy = m_dicList[testName].user_id;
                m_newSolution.drCycleTime = m_dicList[testName].drCycleTime;
                m_newSolution.totalTime = m_dicList[testName].totalTime;
                m_newSolution.description = m_dicList[testName].description;
                m_newSolution.dishName = m_dicList[testName].dish_name;
                m_newSolution.drpower = m_dicList[testName].drpower;
                m_newSolution.utensilName = m_dicList[testName].utensil_name;
                m_newSolution.guid = m_dicList[testName].guid;
                textBox2.Text = m_testInfo.description;
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_userId = m_users[comboBox1.SelectedIndex].ID;
            try
            {
                m_showindexof = 0;
                ShowTests();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            try
            {
                if (textBox1.Text != string.Empty)
                    ShowTestsByNearTestName(textBox1.Text);
                else
                {
                    ShowTests();
                }
            }
            catch (Exception err)
            {

            }
        }
    }
}
