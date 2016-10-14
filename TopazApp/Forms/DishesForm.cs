using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopazApi;

namespace TopazApp.Forms
{
    public partial class DishesForm : Form
    {

        string m_dishName;
        bool close_ok = false;
        List<DISHInfo> m_copyList = new List<DISHInfo>();

        DISHInfo m_selectedDish;

        public DishesForm()
        {
            InitializeComponent();
            Directory.CreateDirectory(@"C:\Goji\TopazPOC\DishPictures");
            button5.Enabled = true;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            label3.Text = "select dish and double click to insert picture";
            label4.Text = "select dish and double click to insert picture";

            try
            {
                List<DISHInfo> list = MySQLConnector.GetAllDishNames();
                foreach (DISHInfo s in list)
                {
                    listBox1.Items.Add(s.dishName);
                    m_copyList.Add(s);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

        }
        public string DishName
        {
            get
            {
                return m_dishName;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty)
            {
                MessageBox.Show("Please select dish name");
                return;
            }
            try
            {

                bool exists = m_copyList.Exists(x => x.dishName == textBox1.Text);
                if (exists == true)
                {
                    MessageBox.Show("Dish category already exists\nPlease select differnt name or use exising");
                    return;
                }

                MySQLConnector.AddNewDishName(textBox1.Text, textBox2.Text);
                m_dishName = textBox1.Text;
                listBox1.Items.Add(m_dishName);
                DISHInfo t = new DISHInfo(textBox1.Text, textBox2.Text);
                m_copyList.Add(t);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           if (listBox1.SelectedIndex != -1)
           {
               button5.Enabled = true;
               m_selectedDish = m_copyList[listBox1.SelectedIndex];
               textBox2.Text = m_selectedDish.description;

               comboBox1.Text = m_selectedDish.dishScoreEnd.ToString();

               pictureBox1.ImageLocation = DRIVE.Drive + @"TopazPOC\DishPictures\" + m_selectedDish.picture1;
               pictureBox2.ImageLocation = DRIVE.Drive + @"TopazPOC\DishPictures\" + m_selectedDish.picture2;
               if (m_selectedDish.picture1 != string.Empty && m_selectedDish.picture1 != null)
                   label3.Text = "";
               else 
                   label3.Text = "select dish and double click to insert picture";

               if (m_selectedDish.picture2 != string.Empty && m_selectedDish.picture2 != null)
                   label4.Text = "";
               else
                   label4.Text = "select dish and double click to insert picture";
           }
           else
           {
               button5.Enabled = false;
           }
        }

        private void button5_Click(object sender, EventArgs e)
        {

            try
            {

                bool found;
                MySQLConnector.UpdateDishDescription(m_selectedDish.dishName, textBox2.Text);
                DISHInfo t = MySQLConnector.GetDishInfo(m_selectedDish.dishName, out found);
                if (m_copyList[listBox1.SelectedIndex].dishName == m_selectedDish.dishName)
                {
                    m_copyList[listBox1.SelectedIndex] = t;
                    comboBox1.Text = t.dishScoreEnd.ToString();
                }
                
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

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                return;
            }

            try
            {
                m_selectedDish = m_copyList[listBox1.SelectedIndex];

                string fileName = GetFileName();
                if (fileName != string.Empty)
                {
                    pictureBox1.ImageLocation = fileName;
                    MySQLConnector.UpdateDishPictureFileName(m_selectedDish.dishName, m_selectedDish.dishName + "_" + Path.GetFileName(fileName), 1);
                    File.Copy(fileName, DRIVE.Drive + @"TopazPOC\DishPictures\" + m_selectedDish.dishName + "_" + Path.GetFileName(fileName), true);
                    
                    File.Copy(fileName, @"c:\Goji\TopazPOC\DishPictures\" + m_selectedDish.dishName + "_" + Path.GetFileName(fileName), true);

                    m_selectedDish.picture1 = m_selectedDish.dishName + "_" + Path.GetFileName(fileName);
                    m_copyList[listBox1.SelectedIndex] = m_selectedDish;
                    label3.Text = "";
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }        
        string GetFileName()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "JPEG Files (.jpg) |*.jpg|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }
            else
            {
                return string.Empty;
            }
        }

        private void pictureBox2_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                return;
            }

            try
            {
                m_selectedDish = m_copyList[listBox1.SelectedIndex];

                string fileName = GetFileName();
                if (fileName != string.Empty)
                {
                    pictureBox2.ImageLocation = fileName;
                    MySQLConnector.UpdateDishPictureFileName(m_selectedDish.dishName, m_selectedDish.dishName + "_" + Path.GetFileName(fileName), 2);
                    File.Copy(fileName, DRIVE.Drive + @"TopazPOC\DishPictures\" + m_selectedDish.dishName + "_" + Path.GetFileName(fileName), true);
                    File.Copy(fileName, @"c:\Goji\TopazPOC\DishPictures\" + m_selectedDish.dishName + "_" + Path.GetFileName(fileName), true);
                    m_selectedDish.picture2 = m_selectedDish.dishName + "_" + Path.GetFileName(fileName);
                    m_copyList[listBox1.SelectedIndex] = m_selectedDish;
                    label4.Text = "";
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            
            if (listBox1.SelectedIndex == -1)
            {
                return;
            }

            try
            {
                 m_selectedDish = m_copyList[listBox1.SelectedIndex];
                 MySQLConnector.UpdateDishScoreEnd(m_selectedDish.dishName, int.Parse(comboBox1.Text));
                 bool found;
                 m_selectedDish = MySQLConnector.GetDishInfo(m_selectedDish.dishName, out found);
                 m_copyList[listBox1.SelectedIndex] = m_selectedDish;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}
