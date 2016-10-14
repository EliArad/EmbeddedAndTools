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
using TopazMultiWCFApp.AlgoParamsForms;
using TopazMultiWCFApp.Forms;

namespace TopazMultiWCFApp
{
    public partial class ScriptBuilder : Form
    {
        
        Dictionary<Tuple<int, string>, Object> m_project = new Dictionary<Tuple<int, string>, Object>();
        ushort m_drCycleTime = 30;
        float m_drPower = 40;
        POCHeating m_heat;



        public ScriptBuilder()
        {
            InitializeComponent();
            m_heat = new POCHeating();
            LoadSettings();
            InitDataGrid();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            try
            {
                LoadFromFile();
                BuildDataGridViewFromLastProject();
            }
            catch (Exception err)
            {

            }

        }
        public class MyValue
        {
            public int id { get; set; }
            public string name { get; set; }
        }
        void InitDataGrid()
        {
            IList<MyValue> values = new List<MyValue> { 
                new MyValue { id = 0, name = "equal energy" }, 
                //new MyValue { id = 1, name = "Gamma Percentage" } ,
                new MyValue { id = 2, name = "RF Off" } ,
                new MyValue { id = 3, name = "DR Treshold" } ,
                new MyValue { id = 4, name = "Top Percentage" } ,
            
            };
            DataGridViewComboBoxColumn col = new DataGridViewComboBoxColumn();
            col.DataSource = values;
            col.DisplayMember = "name";
            col.DataPropertyName = "userid";
            col.ValueMember = "id";
            col.Width = 140;
            dataGridView1.Columns.Add(col);
            col.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            col.FlatStyle = FlatStyle.Popup;
            dataGridView1.Columns[0].Name = "Algo";

            dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Arial", 15.5F, GraphicsUnit.Pixel);


            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[1].Name = "Time";
            dataGridView1.Columns[1].Width = 60;

            dataGridView1.Columns[2].Name = "Power";
            dataGridView1.Columns[2].Width = 55;

            dataGridView1.Columns[3].Name = "Kj";
            dataGridView1.Columns[3].Width = 55;

            dataGridView1.Columns[4].Name = "absorbed";
            dataGridView1.Columns[4].Width = 65;


            dataGridView1.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dataGridView1_EditingControlShowing);


            dataGridView1.ColumnHeadersDefaultCellStyle.Font =
           new Font(dataGridView1.Font, FontStyle.Bold);


        }
        void BuildDataGridViewFromLastProject()
        {

            IList<MyValue> values = new List<MyValue> { 
                new MyValue { id = 0, name = "equal energy" }, 
                //new MyValue { id = 1, name = "Gamma Percentage" } ,
                new MyValue { id = 2, name = "RF Off" }     ,
                new MyValue { id = 3, name = "DR Treshold" } , 
                new MyValue { id = 4, name = "Top Percentage" }             
            };
            int index = 0;
            int rowIndex = 0;
            foreach (KeyValuePair<Tuple<int, string>, Object> t in m_project)
            {

                dataGridView1.Rows.Add();
                switch (t.Key.Item2)
                {
                    case "Gamma Percentage":
                        index = 1;
                        break;
                    case "equal energy":
                        {
                            index = 0;
                            EqualEnergyParams4Gui par = (EqualEnergyParams4Gui)(t.Value);
                            dataGridView1.Rows[rowIndex].Cells[1].Value = TimeSpan.FromSeconds(par.TotalSeconds);
                            dataGridView1.Rows[rowIndex].Cells[2].Value = (float)(par.maxpower);

                            dataGridView1.Rows[rowIndex].Cells[3].Value = (float)(par.targetkj);
                            dataGridView1.Rows[rowIndex].Cells[4].Value = par.absorbed;
                        }
                        break;
                    case "RF Off":
                        {
                            index = 2;
                            RFOffParams par = (RFOffParams)(t.Value);
                            dataGridView1.Rows[rowIndex].Cells[1].Value = TimeSpan.FromSeconds(par.TotalSeconds);
                            dataGridView1.Rows[rowIndex].Cells[2].Value = 0;

                            dataGridView1.Rows[rowIndex].Cells[3].Value = 0;
                            dataGridView1.Rows[rowIndex].Cells[4].Value = 0;
                        }
                        break;
                    case "DR Treshold":
                        {
                            index = 3;
                            DRThresholdParams4Gui par = (DRThresholdParams4Gui)(t.Value);
                            dataGridView1.Rows[rowIndex].Cells[1].Value = TimeSpan.FromSeconds(par.TotalSeconds);
                            dataGridView1.Rows[rowIndex].Cells[2].Value = (float)(par.maxpower);

                            dataGridView1.Rows[rowIndex].Cells[3].Value = (float)(par.targetkj);
                            dataGridView1.Rows[rowIndex].Cells[4].Value = par.absorbed;
                        }
                        break;
                    case "Top Percentage":
                        {
                            index = 4;
                            TopPercentageParams4Gui par = (TopPercentageParams4Gui)(t.Value);
                            dataGridView1.Rows[rowIndex].Cells[1].Value = TimeSpan.FromSeconds(par.TotalSeconds);
                            dataGridView1.Rows[rowIndex].Cells[2].Value = (float)(par.maxpower);

                            dataGridView1.Rows[rowIndex].Cells[3].Value = (float)(par.targetkj);
                            dataGridView1.Rows[rowIndex].Cells[4].Value = par.absorbed;
                        }
                        break;
                    default:
                        {
                            throw (new SystemException("Unknown algo: " + t.Key.Item2));
                        }
                }

                var c = dataGridView1[0, rowIndex] as DataGridViewComboBoxCell;
                if (c == null)
                    return;

                c.DataSource = values;
                c.Value = index;
                c.ValueMember = "id";
                c.DisplayMember = "name";


                rowIndex++;
            }
        }
        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                try
                {
                    TimeSpan time = new TimeSpan(0, 0, 0);
                    if (dataGridView1.Rows[e.RowIndex].Cells["Time"].Value != null)
                    {
                        time = TimeSpan.Parse(dataGridView1.Rows[e.RowIndex].Cells["Time"].Value.ToString());
                    }
                    TimeForm f = new TimeForm(time);
                    f.ShowDialog();
                    time = f.Time;
                    dataGridView1.Rows[e.RowIndex].Cells["Time"].Value = time.ToString();

                    return;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    return;
                }

            }
            else
                if (e.ColumnIndex == 2)
                {
                    try
                    {
                        popupPowerSelectForm f = new popupPowerSelectForm();
                        f.ShowDialog();
                        dataGridView1.Rows[e.RowIndex].Cells["Power"].Value = f.Power.ToString();
                        return;
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                        return;
                    }
                }
                else
                {
                    if (e.ColumnIndex == -1)
                    {
                        if (dataGridView1.Rows[e.RowIndex].Cells["Algo"].Value == null)
                            return;
                        Tuple<int, string> tpl = new Tuple<int, string>(e.RowIndex, dataGridView1.Rows[e.RowIndex].Cells["Algo"].EditedFormattedValue.ToString());

                        SaveProject();

                        if (m_project.ContainsKey(tpl) == false)
                        {
                            MessageBox.Show("Please save before editing algo params");
                            return;
                        }
                        switch (dataGridView1.Rows[e.RowIndex].Cells["Algo"].Value.ToString())
                        {
                            case "0":
                                {
                                    EqualEnergyParams4Gui tpar = (EqualEnergyParams4Gui)m_project[tpl];
                                    EqualEnergyAlgoForm f = new EqualEnergyAlgoForm(tpar, m_drCycleTime);
                                    if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                    {
                                        tpar = (EqualEnergyParams4Gui)f.GetParams();
                                        m_project[tpl] = tpar;
                                    }
                                }
                                break;
                            case "3":
                                {
                                    DRThresholdParams4Gui tpar = (DRThresholdParams4Gui)m_project[tpl];
                                    DRThresholdAlgoForm f = new DRThresholdAlgoForm(tpar, m_drCycleTime);
                                    if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                    {
                                        tpar = (DRThresholdParams4Gui)f.GetParams();
                                        m_project[tpl] = tpar;
                                    }
                                }
                                break;
                            case "4":
                                {
                                    TopPercentageParams4Gui tpar = (TopPercentageParams4Gui)m_project[tpl];
                                    AlgoTopPercentageForm f = new AlgoTopPercentageForm(tpar, m_drCycleTime);
                                    if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                    {
                                        tpar = (TopPercentageParams4Gui)f.GetParams();
                                        m_project[tpl] = tpar;
                                    }
                                }
                                break;
                        }
                    }
                }
        }
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                try
                {
                    Tuple<int, string> tpl = new Tuple<int, string>(index, dataGridView1.Rows[index].Cells["Algo"].EditedFormattedValue.ToString());
                    m_project.Remove(tpl);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                dataGridView1.ClearSelection();
            }
        }
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            ComboBox combo = e.Control as ComboBox;
            if (combo != null)
            {
                combo.SelectedIndexChanged -= new EventHandler(ComboBox_SelectedIndexChanged);
                combo.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
            }
        }
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            string item = cb.Text;

            if (item != null)
            {
                if (item == "RF Off")
                {
                    int index = dataGridView1.CurrentCell.RowIndex;

                    dataGridView1.Rows[index].Cells["kj"].Value = 0;
                    dataGridView1.Rows[index].Cells["power"].Value = 0;
                    dataGridView1.Rows[index].Cells["absorbed"].Value = 0;
                    dataGridView1.Rows[index].Cells["kj"].ReadOnly = true;
                }
                else
                {
                    int index = dataGridView1.CurrentCell.RowIndex;
                    dataGridView1.Rows[index].Cells["kj"].ReadOnly = false;
                }
            }

        }
        void SaveSettings()
        {
            Properties.Settings.Default.DrPower = m_drPower.ToString();
            Properties.Settings.Default.DrCycleTime = m_drCycleTime.ToString();

            Properties.Settings.Default.OveralTimerMin = textBox2.Text;
            Properties.Settings.Default.OveralTimerSec = textBox3.Text;

            Properties.Settings.Default.Save();

        }
        void LoadSettings()
        {
            try
            {
                m_drCycleTime = ushort.Parse(Properties.Settings.Default.DrCycleTime);
                m_drPower = float.Parse(Properties.Settings.Default.DrPower);
                textBox2.Text = Properties.Settings.Default.OveralTimerMin;
                textBox3.Text = Properties.Settings.Default.OveralTimerSec;
            }
            catch (Exception err)
            {

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveProject();
            SaveSettings();

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
        bool SaveProject()
        {
            int index = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if ((dataGridView1.Rows.Count - 1) == index)
                {
                    break;
                }
                TimeSpan t = new TimeSpan(0, 0, 0);
                string algo = string.Empty;
                float maxpower;
                float kj;
                float absorbed;
                try
                {
                    t = TimeSpan.Parse(row.Cells["Time"].Value.ToString());
                }
                catch (Exception er)
                {
                    MessageBox.Show("Time format error in row: " + index);
                    return false;
                }

                try
                {
                    maxpower = float.Parse(row.Cells["Power"].Value.ToString());
                }
                catch (Exception er)
                {
                    MessageBox.Show("Time format error in row: " + index);
                    return false;
                }

                try
                {
                    absorbed = float.Parse(row.Cells["absorbed"].Value.ToString());
                }
                catch (Exception er)
                {
                    MessageBox.Show("absorbed format error in row: " + index);
                    return false;
                }

                try
                {
                    kj = float.Parse(row.Cells["kj"].Value.ToString());
                }
                catch (Exception er)
                {
                    MessageBox.Show("Target KJ format error in row: " + index);
                    return false;
                }

                try
                {
                    algo = row.Cells["Algo"].EditedFormattedValue.ToString();
                    Tuple<int, string> tpl = new Tuple<int, string>(index, algo);

                    switch (algo)
                    {
                        case "Top Percentage":
                            {
                                TopPercentageParams4Gui tpar;
                                if (m_project.ContainsKey(tpl) == false)
                                {
                                    RemoveLastEntry(index);
                                    tpar = new TopPercentageParams4Gui();
                                    tpar.TotalSeconds = (ushort)t.TotalSeconds;
                                    tpar.maxpower = maxpower;
                                    tpar.absorbed = (ushort)absorbed;
                                    tpar.targetkj = (ushort)kj;

                                    m_project.Add(tpl, tpar);
                                }
                                else
                                {
                                    tpar = (TopPercentageParams4Gui)m_project[tpl];
                                    tpar.maxpower = maxpower;
                                    tpar.absorbed = (ushort)absorbed;
                                    tpar.targetkj = (ushort)kj;
                                    tpar.TotalSeconds = (ushort)t.TotalSeconds;
                                    m_project[tpl] = tpar;
                                }
                            }
                            break;
                        case "equal energy":
                            {
                                EqualEnergyParams4Gui tpar;
                                if (m_project.ContainsKey(tpl) == false)
                                {
                                    RemoveLastEntry(index);
                                    tpar = new EqualEnergyParams4Gui();
                                    tpar.TotalSeconds = (ushort)t.TotalSeconds;
                                    tpar.maxpower = maxpower;
                                    tpar.absorbed = (ushort)absorbed;
                                    tpar.targetkj = (ushort)kj;
                                    m_project.Add(tpl, tpar);
                                }
                                else
                                {
                                    tpar = (EqualEnergyParams4Gui)m_project[tpl];
                                    tpar.maxpower = maxpower;
                                    tpar.absorbed = (ushort)absorbed;
                                    tpar.targetkj = (ushort)kj;
                                    tpar.TotalSeconds = (ushort)t.TotalSeconds;
                                    m_project[tpl] = tpar;
                                }
                            }
                            break;
                        case "DR Treshold":
                            {
                                DRThresholdParams4Gui tpar;
                                if (m_project.ContainsKey(tpl) == false)
                                {
                                    RemoveLastEntry(index);
                                    tpar = new DRThresholdParams4Gui();
                                    tpar.TotalSeconds = (ushort)t.TotalSeconds;
                                    tpar.maxpower = maxpower;
                                    tpar.absorbed = (ushort)(absorbed);
                                    tpar.targetkj = (ushort)(kj);

                                    m_project.Add(tpl, tpar);
                                }
                                else
                                {
                                    tpar = (DRThresholdParams4Gui)m_project[tpl];
                                    tpar.maxpower = maxpower;
                                    tpar.absorbed = (ushort)(absorbed);
                                    tpar.targetkj = (ushort)(kj);
                                    tpar.TotalSeconds = (ushort)t.TotalSeconds;
                                    m_project[tpl] = tpar;
                                }
                            }
                            break;
                        case "RF Off":
                            {
                                RFOffParams tpar;
                                if (m_project.ContainsKey(tpl) == false)
                                {
                                    RemoveLastEntry(index);
                                    tpar.TotalSeconds = (ushort)t.TotalSeconds;
                                    m_project.Add(tpl, tpar);
                                }
                                else
                                {
                                    tpar = (RFOffParams)m_project[tpl];
                                    tpar.TotalSeconds = (ushort)t.TotalSeconds;
                                    m_project[tpl] = tpar;
                                }
                            }
                            break;
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("Algo type error in row: " + index);
                    return false;
                }
                index++;
            }
            SaveToFile();
            return true;
        }
        void RemoveLastEntry(int index)
        {
            List<Tuple<int, string>> toRemove = new List<Tuple<int, string>>();
            foreach (KeyValuePair<Tuple<int, string>, Object> p in m_project)
            {
                if (p.Key.Item1 == index)
                {
                    toRemove.Add(p.Key);
                }
            }
            for (int i = 0; i < toRemove.Count; i++)
            {
                m_project.Remove(toRemove[i]);
            }
        }
        void LoadFromFile()
        {
            try
            {
                StreamReader sr = new StreamReader("project.txt");
                int index = 0;
                while (true)
                {
                    string line = sr.ReadLine();
                    if (line == null)
                    {
                        sr.Close();
                        return;
                    }
                    switch (line)
                    {
                        case "Top Percentage":
                            {

                                TopPercentageParams4Gui tpar = new TopPercentageParams4Gui();
                                Tuple<int, string> tpl = new Tuple<int, string>(index, line);
                                tpar.TotalSeconds = ushort.Parse(sr.ReadLine());
                                tpar.agc = bool.Parse(sr.ReadLine());
                                tpar.powertime = ushort.Parse(sr.ReadLine());
                                tpar.equaltime = ushort.Parse(sr.ReadLine());
                                tpar.equalEnergy = bool.Parse(sr.ReadLine());
                                tpar.maxpower = float.Parse(sr.ReadLine());
                                tpar.toppercent = ushort.Parse(sr.ReadLine());
                                tpar.targetkj = float.Parse(sr.ReadLine());
                                tpar.absorbed = float.Parse(sr.ReadLine());

                                m_project.Add(tpl, tpar);
                            }
                            break;
                        case "equal energy":
                            {

                                EqualEnergyParams4Gui epar = new EqualEnergyParams4Gui();
                                Tuple<int, string> tpl = new Tuple<int, string>(index, line);
                                epar.TotalSeconds = ushort.Parse(sr.ReadLine());
                                epar.agc = bool.Parse(sr.ReadLine());
                                epar.mode = ushort.Parse(sr.ReadLine());
                                epar.highvalue = float.Parse(sr.ReadLine());
                                epar.lowvalue = float.Parse(sr.ReadLine());
                                epar.maxpower = float.Parse(sr.ReadLine());
                                epar.acckj = float.Parse(sr.ReadLine());
                                epar.targetkj = ushort.Parse(sr.ReadLine());
                                epar.absorbed = ushort.Parse(sr.ReadLine());
                                epar.singlerepetition = bool.Parse(sr.ReadLine());
                                epar.toppercentage = ushort.Parse(sr.ReadLine());
                                m_project.Add(tpl, epar);
                            }
                            break;
                        case "DR Treshold":
                            {
                                DRThresholdParams4Gui dpar = new DRThresholdParams4Gui();
                                Tuple<int, string> tpl = new Tuple<int, string>(index, line);
                                dpar.TotalSeconds = ushort.Parse(sr.ReadLine());
                                dpar.agc = bool.Parse(sr.ReadLine());
                                dpar.mode = ushort.Parse(sr.ReadLine());
                                dpar.highvalue = float.Parse(sr.ReadLine());
                                dpar.lowvalue = float.Parse(sr.ReadLine());
                                dpar.powertime = ushort.Parse(sr.ReadLine());
                                dpar.maxpower = float.Parse(sr.ReadLine());
                                dpar.targetkj = float.Parse(sr.ReadLine());
                                dpar.absorbed = float.Parse(sr.ReadLine());
                                dpar.equaldrtime = bool.Parse(sr.ReadLine());
                                m_project.Add(tpl, dpar);
                            }
                            break;
                        case "RF Off":
                            {
                                RFOffParams rfpar = new RFOffParams();
                                Tuple<int, string> tpl = new Tuple<int, string>(index, line);
                                rfpar.TotalSeconds = ushort.Parse(sr.ReadLine());
                                m_project.Add(tpl, rfpar);
                            }
                            break;
                    }
                    index++;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("LoadFromFile: " + err.Message);
            }
        }

        bool BuildAlgoFromUserDataGridView()
        {
            int index = 0;
            m_heat.Clear();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if ((dataGridView1.Rows.Count - 1) == index)
                {
                    break;
                }
                TimeSpan t = new TimeSpan(0, 0, 0);
                string algo = string.Empty;
                float maxpower;
                float kj;
                float absorbed;
                try
                {
                    t = TimeSpan.Parse(row.Cells["Time"].Value.ToString());
                }
                catch (Exception er)
                {
                    MessageBox.Show("Time format error in row: " + index);
                    return false;
                }

                try
                {
                    maxpower = float.Parse(row.Cells["Power"].Value.ToString());
                }
                catch (Exception er)
                {
                    MessageBox.Show("Time format error in row: " + index);
                    return false;
                }

                try
                {
                    absorbed = float.Parse(row.Cells["absorbed"].Value.ToString());
                }
                catch (Exception er)
                {
                    MessageBox.Show("absorbed format error in row: " + index);
                    return false;
                }

                try
                {
                    kj = float.Parse(row.Cells["kj"].Value.ToString());
                }
                catch (Exception er)
                {
                    MessageBox.Show("Target KJ format error in row: " + index);
                    return false;
                }

                try
                {
                    algo = row.Cells["Algo"].EditedFormattedValue.ToString();
                    Tuple<int, string> tpl = new Tuple<int, string>(index, algo);

                    switch (algo)
                    {
                        case "Top Percentage":
                            {
                                TopPercentageParams par = new TopPercentageParams();
                                TopPercentageParams4Gui tpar;
                                tpar = (TopPercentageParams4Gui)m_project[tpl];
                                par.TotalSeconds = (ushort)t.TotalSeconds;
                                par.maxpower = (ushort)(maxpower * 10);
                                par.absorbed = (ushort)(absorbed);
                                par.targetkj = (ushort)(kj * 10);
                                par.agc = (ushort)(tpar.agc == true ? 1 : 0);
                                par.powertime = tpar.powertime;
                                par.equaltime = tpar.equaltime;

                                par.toppercent = tpar.toppercent;
                                if (par.toppercent == 0)
                                {
                                    MessageBox.Show("Top percentage cannot be zero");
                                    return false;
                                }

                                par.absorbed = (ushort)tpar.absorbed;
                                par.equalEnergy = (ushort)(tpar.equalEnergy == true ? 1 : 0);

                                m_heat.AddAlgo(ALGO_TYPES.TOP_PERCENTAGE, par, null, m_drCycleTime, (ushort)(m_drPower * 10));
                            }
                            break;
                        case "equal energy":
                            {
                                EqualEnergyParams tpar = new EqualEnergyParams();
                                EqualEnergyParams4Gui tgpar;
                                tgpar = (EqualEnergyParams4Gui)m_project[tpl];
                                tpar.TotalSeconds = (ushort)t.TotalSeconds;
                                tpar.maxpower = (ushort)(maxpower * 10);
                                tpar.absorbed = (ushort)(absorbed);
                                tpar.targetkj = (ushort)(kj * 10);

                                tpar.highvalue = (ushort)(tgpar.highvalue * 10);
                                tpar.lowvalue = (ushort)(tgpar.lowvalue * 10);
                                tpar.highvalue = (ushort)(tgpar.highvalue * 10);
                                tpar.mode = tgpar.mode;
                                tpar.agc = (ushort)(tgpar.agc == true ? 1 : 0);
                                tpar.toppercentage = tgpar.toppercentage;
                                tpar.singlerepetition = (ushort)(tgpar.singlerepetition == true ? 1 : 0);
                                tpar.acckj = (ushort)(tgpar.acckj * 10);

                                m_heat.AddAlgo(ALGO_TYPES.EQUAL_ENERGY, tpar, null, m_drCycleTime, 47 * 10);
                            }
                            break;
                        case "DR Treshold":
                            {
                                DRThresholdParams4Gui d4par;
                                DRThresholdParams dpar = new DRThresholdParams();
                                d4par = (DRThresholdParams4Gui)m_project[tpl];
                                dpar.TotalSeconds = (ushort)t.TotalSeconds;
                                dpar.maxpower = (ushort)(maxpower * 10);
                                dpar.absorbed = (ushort)(absorbed);
                                dpar.targetkj = (ushort)(kj * 10);

                                dpar.agc = (ushort)(d4par.agc == true ? 1 : 0);
                                dpar.mode = d4par.mode;
                                dpar.highvalue = (ushort)(d4par.highvalue * 10);
                                dpar.lowvalue = (ushort)(d4par.lowvalue * 10);
                                dpar.powertime = d4par.powertime;
                                dpar.equaldrtime = (ushort)(d4par.equaldrtime == true ? 1 : 0);

                                m_heat.AddAlgo(ALGO_TYPES.DR_THREASHOLDS, dpar, null, null, m_drCycleTime, 47 * 10);
                            }
                            break;
                        case "RF Off":
                            {
                                RFOffParams rfpar;
                                rfpar = (RFOffParams)m_project[tpl];
                                m_heat.AddAlgo(ALGO_TYPES.HEAT_OFF, (ushort)t.TotalSeconds);
                            }
                            break;
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("Algo type error in row: " + index);
                    return false;
                }
                index++;
            }
            return true;
        }
        void SaveToFile()
        {

            StreamWriter sw = new StreamWriter("project.txt");
            foreach (KeyValuePair<Tuple<int, string>, Object> d in m_project)
            {
                sw.WriteLine(d.Key.Item2);
                switch (d.Key.Item2)
                {
                    case "Top Percentage":
                        {
                            TopPercentageParams4Gui par = (TopPercentageParams4Gui)d.Value;
                            sw.WriteLine(par.TotalSeconds);
                            sw.WriteLine(par.agc);
                            sw.WriteLine(par.powertime);
                            sw.WriteLine(par.equaltime);
                            sw.WriteLine(par.equalEnergy);
                            sw.WriteLine(par.maxpower);
                            sw.WriteLine(par.toppercent);
                            sw.WriteLine(par.targetkj);
                            sw.WriteLine(par.absorbed);
                        }
                        break;
                    case "equal energy":
                        {
                            EqualEnergyParams4Gui par = (EqualEnergyParams4Gui)d.Value;
                            sw.WriteLine(par.TotalSeconds);
                            sw.WriteLine(par.agc);
                            sw.WriteLine(par.mode);
                            sw.WriteLine(par.highvalue);
                            sw.WriteLine(par.lowvalue);
                            sw.WriteLine(par.maxpower);
                            sw.WriteLine(par.acckj);
                            sw.WriteLine(par.targetkj);
                            sw.WriteLine(par.absorbed);
                            sw.WriteLine(par.singlerepetition);
                            sw.WriteLine(par.toppercentage);
                        }
                        break;
                    case "DR Treshold":
                        {
                            DRThresholdParams4Gui par = (DRThresholdParams4Gui)d.Value;
                            sw.WriteLine(par.TotalSeconds);
                            sw.WriteLine(par.agc);
                            sw.WriteLine(par.mode);
                            sw.WriteLine(par.highvalue);
                            sw.WriteLine(par.lowvalue);
                            sw.WriteLine(par.powertime);
                            sw.WriteLine(par.maxpower);
                            sw.WriteLine(par.targetkj);
                            sw.WriteLine(par.absorbed);
                            sw.WriteLine(par.equaldrtime);
                        }
                        break;
                    case "RF Off":
                        {
                            RFOffParams par = (RFOffParams)d.Value;
                            sw.WriteLine(par.TotalSeconds);
                        }
                        break;
                }
            }
            sw.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == false)
            {
                textBox2.Text = "0";
                textBox3.Text = "0";
                textBox2.Enabled = false;
                textBox3.Enabled = false;
            }
            else
            {
                textBox2.Enabled = true;
                textBox3.Enabled = true;
            }
        }

    }
}
