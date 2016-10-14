using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Description;
using RestApi.HttpUtils;
using TopazControllerLib;
using System.IO;
using TopazApi;
using System.Threading;
 
namespace TopazMultiWCFApp
{

    public enum API_STARTING_MODE
    {
        BY_PARAMS,
        BY_DISH
    }

    public struct EqualEnergyParams4Gui
    {
        public ushort TotalSeconds;
        public bool agc;
        public ushort mode;
        public float highvalue;
        public float lowvalue;
        public float maxpower;
        public float acckj;
        public ushort targetkj;
        public float absorbed;
        public bool singlerepetition;
        public ushort toppercentage;
    }

    public struct TopPercentageParams4Gui
    {
        public ushort TotalSeconds;
        public bool agc;
        public ushort powertime;
        public ushort equaltime;
        public float maxpower;
        public ushort toppercent;
        public float targetkj;
        public float absorbed;
        public bool equalEnergy;
    }


    public struct DRThresholdParams4Gui
    {
        public ushort TotalSeconds;
        public bool agc;
        public ushort mode;
        public float highvalue;
        public float lowvalue;
        public ushort powertime;
        public float maxpower;
        public float targetkj;
        public float absorbed;
        public bool equaldrtime;
    }

    public partial class TopazWCFControl : UserControl
    {
        bool m_pause = false;
        Dictionary<Tuple<int, string>, Object> m_project = new Dictionary<Tuple<int, string>, Object>();
        TopazRestApi m_client = new TopazRestApi();
        POCHeating m_heat = new POCHeating();
        ushort m_drCycleTime = 30;
        float m_drPower = 40;
        bool m_activeApp = true;
        int m_totalMin = 0;
        int m_totalSec = 0;
        Thread m_statusThread;
        int m_cycleCount = 0;
        public TopazWCFControl()
        {
            InitializeComponent();
            GuidEnable(false);
            Control.CheckForIllegalCrossThreadCalls = false;
            
        }
        void StatusThread()
        {
            while (m_activeApp)
            {
                try
                {
                    StatusResponse status = m_client.ReadStatus();
                    sskjtotal.Value = status.kj.ToString("0.000");
                    ssMin.Value = status.clockMinutes.ToString("00");
                    ssSec.Value = status.clockSeconds.ToString("00");
                    TimeSpan t = TimeSpan.FromSeconds(status.AlgoClockSeconds);
                    sevenSegmentArray2.Value = t.Minutes.ToString("00");
                    sevenSegmentArray1.Value = t.Seconds.ToString("00");
                    label14.Text = status.drCount.ToString();
                    if (status.cycleEndded == 1)
                        label9.Text = (++m_cycleCount).ToString();
                    
                }
                catch (Exception err)
                {
                    label15.Text = err.Message;
                }
                Thread.Sleep(1000);
            }
        }
        public void SetWebAddress(string ipAddress)
        {
            string url = "http://" + ipAddress + ":8000";
            m_client.SetHttpUrl(url, 0);
        }
        public bool Connect()
        {
            try
            {
                try
                {
                    LoadSettings();
                    LoadFromFile();
                }
                catch (Exception err)
                {

                }
                m_client.connect(string.Empty, 0);
                GuidEnable(true);
                m_statusThread = new Thread(StatusThread);
                m_statusThread.Start();
                return true;
            }
            catch (Exception err)
            {
                return false;
            }
        }
        void GuidEnable(bool e)
        {
            btnPause.Enabled = e;
            btnStart.Enabled = e;
            btnStop.Enabled = e;
        }
        void LoadSettings()
        {
            try
            {
                m_drCycleTime = ushort.Parse(Properties.Settings.Default.DrCycleTime);
                m_drPower = float.Parse(Properties.Settings.Default.DrPower);
                m_totalMin = int.Parse(Properties.Settings.Default.OveralTimerMin);
                m_totalSec = int.Parse(Properties.Settings.Default.OveralTimerSec);
            }
            catch (Exception err)
            {

            }

        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                m_cycleCount = 0;
                LoadFromFile();
                if (m_project.Count == 0)
                {
                    MessageBox.Show("Script not loaded yet");
                    return;
                }
                TimeSpan time = new TimeSpan(0, m_totalMin, m_totalSec);
                BuildAlgoProjectDictionary();
                m_client.SendDataToController(m_heat.GetList());
                m_client.Start((ushort)time.TotalSeconds, true);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                m_client.Stop();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        public void Close()
        {
            m_activeApp = false;
            if (m_statusThread != null)
                m_statusThread.Join();
        }
        void LoadFromFile()
        {
            try
            {
                m_project.Clear();
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

        private void btnPause_Click(object sender, EventArgs e)
        {
            try
            {
                m_pause = !m_pause;
                m_client.Pause(m_pause);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

       
        private void programNewDishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScriptBuilder s = new ScriptBuilder();
            if (s.ShowDialog() == DialogResult.OK)
            {

            }
        }
        bool BuildAlgoProjectDictionary()
        {
            int index = 0;
            m_heat.Clear();
            foreach (KeyValuePair<Tuple<int, string>, Object> d in m_project)
            {
                 
                try
                {
                    string algo = d.Key.Item2;
                    Tuple<int, string> tpl = new Tuple<int, string>(index, algo);

                    switch (algo)
                    {
                        case "Top Percentage":
                            {
                                TopPercentageParams par = new TopPercentageParams();
                                TopPercentageParams4Gui tpar;
                                tpar = (TopPercentageParams4Gui)m_project[tpl];
                                par.TotalSeconds = tpar.TotalSeconds;
                                par.maxpower = (ushort)(tpar.maxpower * 10);
                                par.absorbed = (ushort)(tpar.absorbed);
                                par.targetkj = (ushort)(tpar.targetkj * 10);
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
                                tpar.TotalSeconds = (ushort)tgpar.TotalSeconds;
                                tpar.maxpower = (ushort)(tgpar.maxpower * 10);
                                tpar.absorbed = (ushort)(tgpar.absorbed);
                                tpar.targetkj = (ushort)(tgpar.targetkj * 10);

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
                                dpar.TotalSeconds = (ushort)d4par.TotalSeconds;
                                dpar.maxpower = (ushort)(d4par.maxpower * 10);
                                dpar.absorbed = (ushort)(d4par.absorbed);
                                dpar.targetkj = (ushort)(d4par.targetkj * 10);

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
                                m_heat.AddAlgo(ALGO_TYPES.HEAT_OFF, (ushort)rfpar.TotalSeconds);
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
    }
}
