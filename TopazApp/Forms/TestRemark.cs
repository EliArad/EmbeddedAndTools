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
    public partial class TestRemark : Form
    {
        PictureInfo [] m_pictures = new PictureInfo[4];
       
        string m_remark;
        string m_testName;
        int m_userId;
        bool m_before;
        int m_testId;
        AddPictures m_addPictures;
        TestInfo? m_lastTest = null;
        string m_compareReason = "";
        int m_compareTo = -1;
        int m_loadedTestId = -1;
        public TestRemark(int userid, 
                          bool before, 
                          int testId, 
                          TestInfo ? lastTest,
                          string groupname, 
                          int loadedTestId)
        {
            InitializeComponent();
            m_loadedTestId = loadedTestId;
            m_lastTest = lastTest;
            m_userId = userid;
            m_before = before;
            m_testId = testId;
            m_addPictures = new AddPictures(m_userId, m_before, m_testId);
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            if (m_lastTest != null)
            {
                textBox3.Text = m_lastTest.Value.testname + " (" + m_lastTest.Value.id + " )";
            }

            linkLabel2.Text = "Group name: " + groupname;
        }

        public string Remark
        {
            get
            {
                return m_remark;
            }
        }

        public string CompareReason
        {
            get
            {
                return m_compareReason;
            }
        }
        public int CompareTo
        {
            get
            {
                return m_compareTo;
            }
        }
        public string TestName
        {
            get
            {
                return m_testName;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty)
            {
                MessageBox.Show("Please fill description for this test");
                return;
            }

            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select the compare reason");
                return;
            }
            if (comboBox1.SelectedIndex == 0)
            {
                m_compareTo = m_testId;
            }
            else if (comboBox1.SelectedIndex == 4)
            {
                m_compareTo = m_loadedTestId;
            }
            else
            {
                m_compareTo = m_lastTest.Value.id;
            }

            if (m_addPictures.AddedPic == 0)
            {
                DialogResult d = MessageBox.Show("Do you want to save picture before the test begin?" + Environment.NewLine + "In case you choose no , you can attache picture later on", "POC is the future", MessageBoxButtons.YesNo);
                if (d == System.Windows.Forms.DialogResult.Yes)
                {
                    m_addPictures.ShowDialog();
                }
            }

            m_compareReason = comboBox1.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            m_remark = textBox1.Text;
            m_testName = textBox2.Text;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
        public PictureInfo [] GetPicturesList()
        {
            return m_pictures;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            m_addPictures.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoadTest f = new LoadTest(m_userId);
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TestInfo t = f.TestInformation;
                textBox3.Text = t.testname + " (" + t.id + " )";
                m_lastTest = t;
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GroupNames f = new GroupNames();
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                linkLabel2.Text = "Group name: " + f.GroupName;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
