using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopazApp.Forms;
using TopazApp.UserControls;

namespace TopazApp
{
    public partial class BuildChefDemo : Form
    {
        List<TestInfo> m_testList;
        List<Tuple<int, string>> m_testIdList = new List<Tuple<int, string>>();
        bool m_dirty = false;
        public BuildChefDemo()
        {
            InitializeComponent();
            InitDataGrid();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            List<string> l = MySQLConnector.GetGroupNames();

            foreach (string s in l)
            {
                comboBox1.Items.Add(s);
            }
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
                List<ChefDemoDishes> l1 = MySQLConnector.GetChefDemoList(comboBox1.Text);
                foreach (ChefDemoDishes x in l1)
                {
                    dataGridView1.Rows.Add(x.testName, x.testid, x.dish_name, x.finalDescription);
                }
            }

            comboBox1.Text = Properties.Settings.Default.ChefDemoGroupName;

        }
        void SelectedTestFunc(TestInfo t)
        {
            m_dirty = true;
            dataGridView1.Rows.Add(t.testname, t.id, t.dish_name, t.fres.FinalDescription);

        }

        bool BuildListFromGrid()
        {

            m_testIdList.Clear();
            int index = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if ((dataGridView1.Rows.Count - 1) == index)
                {
                    break;
                }
                 
                try
                {
                    int id = int.Parse(row.Cells["Test Id"].Value.ToString());
                    string desc = row.Cells["description"].Value.ToString();
                    Tuple<int, string> t = new Tuple<int, string>(id, desc);
                    m_testIdList.Add(t);
                }
                catch (Exception er)
                {
                    MessageBox.Show("Time format error in row: " + index);
                    return false;
                }
                index++;
            }
            return true;
        }
        void InitDataGrid()
        {
            
             
            dataGridView1.ColumnCount = 4;
            dataGridView1.Columns[0].Name = "Test name";
            dataGridView1.Columns[0].Width = 60;

            dataGridView1.Columns[1].Name = "Test Id";
            dataGridView1.Columns[1].Width = 55;

            dataGridView1.Columns[2].Name = "Dish name";
            dataGridView1.Columns[2].Width = 55;

            dataGridView1.Columns[3].Name = "description";
            dataGridView1.Columns[3].Width = 255;

       
            dataGridView1.ColumnHeadersDefaultCellStyle.Font =
           new Font(dataGridView1.Font, FontStyle.Bold);


        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty)
            {
                panel1.Controls.Clear();
                return;
            }
            m_testList = MySQLConnector.GetTestListByNearTestName(-1, "test name", ASCD.ASC, textBox1.Text, 0, 20);

            panel1.Controls.Clear();
            for (int i = 0; i < m_testList.Count; i++)
            {
                TestListControl.SelectedTest p = new TestListControl.SelectedTest(SelectedTestFunc);
                TestListControl test = new TestListControl(p);

                test.Location = new System.Drawing.Point(10, 0 + i * 100);
                test.Name = test.Name + i.ToString();
                test.Size = new System.Drawing.Size(800, 100);
                test.setTest(m_testList[i]);
                panel1.Controls.Add(test);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == string.Empty)
            {
                MessageBox.Show("Please select group name");
                return;
            
            }
            //if (m_dirty == true)
            {
                BuildListFromGrid();
                if (m_testIdList.Count > 0)
                    MySQLConnector.BuildChefList(m_testIdList, comboBox1.Text);
            }

            Properties.Settings.Default.ChefDemoGroupName = comboBox1.Text;
            Properties.Settings.Default.Save();

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GroupNames f = new GroupNames();
            f.ShowDialog();
            comboBox1.Items.Clear();
            List<string> l = MySQLConnector.GetGroupNames();

            foreach (string s in l)
            {
                comboBox1.Items.Add(s);
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count > 0)
            {
                
                List<ChefDemoDishes> l = MySQLConnector.GetChefDemoList(comboBox1.Text);
                foreach (ChefDemoDishes x in l)
                {
                    dataGridView1.Rows.Add(x.testName, x.testid, x.dish_name, x.description);
                }
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                m_dirty = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text == string.Empty)
            {
                panel1.Controls.Clear();
                return;
            }
            int testid;
            bool b = int.TryParse(textBox2.Text, out testid);
            if (b == false)
                return;
            TestInfo testInfo = MySQLConnector.GetTestInfo(testid);

            panel1.Controls.Clear();
            TestListControl.SelectedTest p = new TestListControl.SelectedTest(SelectedTestFunc);
            TestListControl test = new TestListControl(p);

            test.Location = new System.Drawing.Point(10, 0 + 0 * 100);
            test.Name = test.Name + "1";
            test.Size = new System.Drawing.Size(800, 100);
            test.setTest(testInfo);
            panel1.Controls.Add(test);
            
        }
    }
}
