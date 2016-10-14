using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace TopazApp
{
    public partial class ListUsersOperators : Form
    {
        usersDb u = new usersDb();

        User m_currentSelectedUser;
        public ListUsersOperators()
        {
            InitializeComponent();

            try
            {
                dataGridView1.ColumnCount = 6;
                dataGridView1.Columns[0].Name = "ID";
                dataGridView1.Columns[1].Name = "user name";
                dataGridView1.Columns[2].Name = "First Name";
                dataGridView1.Columns[3].Name = "Last Name";
                dataGridView1.Columns[4].Name = "Phone number";
                dataGridView1.Columns[5].Name = "Active";

                ShowUsers();
               
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        void ShowUsers()
        {
            try
            {
                this.dataGridView1.Rows.Clear();
                List<User> l = u.ReadAllUsers();
                if (l != null)
                {
                    for (int i = 0; i < l.Count; i++)
                    {
                        User x = l[i];
                        this.dataGridView1.Rows.Add(x.ID, x.email, x.firstName, x.lastName, x.phoneNumber, x.active);
                    }
                }
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
             

            try
            {/*
                Users nu = new Users();

                //string s = listBox1.Items[listBox1.SelectedIndex].ToString();
                string[] splitedLine = s.Split(new Char[] { ',' });

                User u = new User
                {
                    typeOfUser = splitedLine[1].Trim() == "Technician" ? 1 : 0,
                    userName = splitedLine[0],
                    active = splitedLine[2] == "active" ? true : false
                };
             
                nu.DeleteUser(u);
                MessageBox.Show("User deleted");*/
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            usersDb nu = new usersDb();
              
            try
            {
                nu.SuspendUser(m_currentSelectedUser.ID, !m_currentSelectedUser.active);
                ShowUsers();
                MessageBox.Show("User suspended");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (m_currentSelectedUser != null)
            {
                SaveUserChangesForm f = new SaveUserChangesForm(m_currentSelectedUser);
                f.ShowDialog();
                ShowUsers();
            }
            else
            {
                MessageBox.Show("Please select a row to edit");
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                CreateUser f = new CreateUser();
                f.ShowDialog();           
                ShowUsers();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                ShowUsers();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;// get the Row Index
            if (index > -1)
            {
                m_currentSelectedUser = new User();
                DataGridViewRow selectedRow = dataGridView1.Rows[index];
                m_currentSelectedUser.ID = int.Parse(selectedRow.Cells[0].Value.ToString());
                m_currentSelectedUser.email = selectedRow.Cells[1].Value.ToString();
                m_currentSelectedUser.firstName = selectedRow.Cells[2].Value.ToString();
                m_currentSelectedUser.lastName = selectedRow.Cells[3].Value.ToString();
                m_currentSelectedUser.phoneNumber = selectedRow.Cells[4].Value.ToString();
                m_currentSelectedUser.active = selectedRow.Cells[5].Value.ToString() == "True" ? true : false;
            }
            else
            {
                m_currentSelectedUser = null;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (m_currentSelectedUser != null)
            {
                DialogResult d = MessageBox.Show("Do you want to delete this user?", "PA ATE", MessageBoxButtons.YesNo);
                if (d == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        usersDb nu = new usersDb();
                        nu.DeleteUser(m_currentSelectedUser.ID);
                        ShowUsers();
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                    }
                }

            }
        }
    }
}
