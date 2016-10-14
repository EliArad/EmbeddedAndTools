using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopazApp.MYSQL;

namespace TopazApp.TOPNAT
{
    public partial class QuerisForm : Form
    {
        List<QuerisInfo> m_listQueris;
        Dictionary<string, QuerisInfo> m_dicQueris = new Dictionary<string, QuerisInfo>();
        public QuerisForm()
        {
            InitializeComponent();

            InitTreeViewQueris();
        }

        void InitTreeViewQueris()
        {

            m_listQueris = MySQLConnector.GetAllQueris();
            List<TreeNode> nodeList = new List<TreeNode>();
            foreach (QuerisInfo q in m_listQueris)
            {
                TreeNode node = new TreeNode(q.queryname);
                nodeList.Add(node);
                m_dicQueris.Add(q.queryname, q);
            }

            TreeNode treeNode = new TreeNode("Topaz SQL Queris", nodeList.ToArray());
            treeView1.Nodes.Add(treeNode);

        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (m_dicQueris.ContainsKey(e.Node.Text))
            {
                try
                {
                    QuerisInfo q = m_dicQueris[e.Node.Text];
                    InitDataGridView(q.datagrid_column);
                    RunQuery(q.sqlquery);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }            
        }
        void RunQuery(string sql)
        {
            try
            {
                List<EfficiencyCompareQueryResult> l = Queris.RunCompareEfficiencyQuery(sql);

                foreach (EfficiencyCompareQueryResult s in l)
                {

                    var index = dataGridView1.Rows.Add();

                     

                    foreach (var field in typeof(EfficiencyCompareQueryResult).GetFields(BindingFlags.Instance |
                                                          BindingFlags.NonPublic |
                                                          BindingFlags.Public))
                    {

                         
                        Column<object> val = (Column<object>)field.GetValue(s);
                        dataGridView1.Rows[index].Cells[field.Name].Value = val.value.ToString();                          
                                           
                    }
                }
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
            
        }
        void InitDataGridView(string cname)
        {
            string[] names = cname.Split(new Char[] { ',' });

            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();

            dataGridView1.ColumnCount = names.Length;
            int i = 0;
            foreach (string s in names)
            {
                string ns = s;
                ns = ns.Trim(' ');
                ns = ns.Trim('\n');
                ns = ns.Trim('\r');
                ns = ns.Trim(' ');
                dataGridView1.Columns[i].Name = ns;
                dataGridView1.Columns[i].Width = 80;
                i++;
            } 
        }
    }
}
