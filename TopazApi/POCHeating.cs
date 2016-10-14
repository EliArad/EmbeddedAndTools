using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TopazCommonDefs;
using TopazControllerLib;

namespace TopazApi
{
    public delegate void CallbackMessage(int code, string msg);
    public delegate void DRValueCallbackMsg(DR [] values);
    public delegate void EQNGValueCallbackMsg(EnergyData[] values);
    public delegate void CallbackAlgo(int code, string msg);

     
    public struct DRThresholdParams
    {
        public ushort TotalSeconds;
        public ushort agc;
        public ushort mode;
        public ushort highvalue;
        public ushort lowvalue;
        public ushort powertime;
        public ushort maxpower;
        public ushort targetkj;
        public ushort absorbed;
        public ushort equaldrtime;
    }

    public struct RFOffParams
    {
        public ushort TotalSeconds;
    }
    public struct EqualEnergyParams
    {
        public ushort TotalSeconds;
        public ushort agc;
        public ushort mode;
        public ushort highvalue;
        public ushort lowvalue;
        public ushort maxpower;
        public ushort acckj;
        public ushort targetkj;
        public ushort absorbed;
        public ushort singlerepetition;
        public ushort toppercentage;            
    }

    public struct TopPercentageParams
    {
        public ushort TotalSeconds;
        public ushort agc;
        public ushort powertime;
        public ushort equaltime;
        public ushort maxpower;
        public ushort toppercent;
        public ushort targetkj;
        public ushort absorbed;
        public ushort equalEnergy;
    }

    public enum DRMODES
    {
        HIGHER_THEN_DR,
        LOWER_THEN_DR,
        BETWEEN_DR_RANGE
    }

    public enum ALGO_TYPES
    {
        EQUAL_ENERGY,
        DR_THREASHOLDS,
        TOP_PERCENTAGE,
        HEAT_OFF,
    }
    public struct DRRange
    {
        public float drlow;
        public float drhigh;
        public int percentage;
    }
     

    public static class DRIVE
    {
        public static string Drive = @"C:\Goji\";
    }
    public class POCHeating 
    {
        byte m_lastRowIndex = 255;
        Thread m_totalCookingThread;
        Thread m_cookingResponseThread;
        AutoResetEvent m_totalTimeEvent = new AutoResetEvent(false);
        protected ManualResetEvent m_manualPauseEvent = new ManualResetEvent(true);
        protected DateTime m_pauseStart;
        protected List<TimeSpan> m_pauseTime = new List<TimeSpan>();
        protected EQNGValueCallbackMsg m_EqngCallback;
        public delegate void CallbackAlgo (int code, string msg);
        protected Queue<Tuple<DateTime, double>> m_totalKjQueue = new Queue<Tuple<DateTime, double>>();

        List<ushort> m_list = new List<ushort>();

        Commands m_commands = null;
        protected double m_drPower;
        protected CallbackMessage m_directCallback;
        protected AutoResetEvent m_sleep = new AutoResetEvent(false);
        protected Thread m_thread;
        protected bool m_running = false;
        protected double m_totalKj;
        protected CallbackMessage     pCallback;
        protected int m_channel = 0;
        protected DRValueCallbackMsg m_DrCallback;
        ITopazControllerApi m_con;
        Thread m_connectionThread;
        bool m_connected = false;
        bool m_tryConnect = true;
        bool m_mbedCommPortNumber = false;
        public enum I2C_MASTER_COMPANY
        {
            DIOLAN,
            USBISS
        }
        
        public POCHeating()
        {

        }
        public void AllowAutoResetOnError()
        {
            m_mbedCommPortNumber = true;
        }
        public POCHeating(I2C_MASTER_COMPANY of = I2C_MASTER_COMPANY.DIOLAN)
        {
            

            Directory.CreateDirectory(DRIVE.Drive + @"TopazPOC\\Energy");
            Directory.CreateDirectory(DRIVE.Drive + @"TopazPOC\\DR");
            Directory.CreateDirectory(DRIVE.Drive + @"TopazPOC\\Power");

            if (of == I2C_MASTER_COMPANY.DIOLAN)
                m_con = new DiolanController();
            else
                m_con = new USBIssController();
            m_connectionThread = new Thread(Connection);
            m_connectionThread.Start();
          
        }

        void Connection()
        {
            while (m_tryConnect == true)
            {
                try
                {
                    m_con.connect(string.Empty, 115200, true);
                    m_commands = new Commands(m_con);
                    m_connected = true;
                    m_cookingResponseThread = new Thread(CookingResponseThread);
                    m_cookingResponseThread.Start();
                    break;
                }
                catch (Exception err)
                {
                    if (pCallback != null)
                        pCallback(400, err.Message);
                    Thread.Sleep(1000);
                }
            }
        }
        public Commands Command
        {
            get
            {
                return m_commands;
            }
        }
        public bool Connected
        {
            get
            {
                return m_connected;
            }
        }
        
        public bool Running
        {
            get
            {
                return m_running;
            }
        }
        public void SetCallback(CallbackMessage p)                               
        {
            pCallback = p;
        }
         
        public void Pause(bool paused)
        {
            m_con.Pause(paused);             
        }
        public void Start(bool onetime, 
                          TimeSpan? totalRunningTime)
        {

            ushort tsec = 0;
            m_lastRowIndex = 255;
            if (totalRunningTime != null)
                tsec = (ushort)(totalRunningTime.Value.TotalSeconds);            
            m_con.Start(tsec, onetime);
        }
        public void Start(ushort dishId)
        {
            m_lastRowIndex = 255;
            m_con.StartDish(dishId);
        }
        public void Stop()
        {
            m_con.Stop();
        }
        protected void wc(int code , string msg)
        {
            if (pCallback != null)
                pCallback(code, msg);
        }
        
        public void SetDirectCallback(CallbackMessage p)
        {

        }

        public void AddAlgo(ALGO_TYPES algoType, 
                            EqualEnergyParams par, 
                            List<DRRange> l, 
                            bool[] freqs,
                            ushort drCycleTime,
                            ushort  drPower)
        {


            m_list.Add((ushort)algoType);
            m_list.Add(drCycleTime);                    
            m_list.Add(drPower);

            m_list.Add(par.TotalSeconds);
            m_list.Add(par.agc);
            m_list.Add(par.mode);
            m_list.Add(par.highvalue);
            m_list.Add(par.lowvalue);
            m_list.Add(par.maxpower);
            m_list.Add(par.acckj);
            m_list.Add(par.targetkj);
            m_list.Add(par.absorbed);
            m_list.Add(par.singlerepetition);
            m_list.Add(par.toppercentage);
             
                
            // Set the count of how many dr range we have , we could have zero
            if (l != null)
            {
                m_list.Add((ushort)l.Count);
                foreach (DRRange drange in l)
                {
                    m_list.Add((ushort)(drange.drhigh * 10));
                    m_list.Add((ushort)(drange.drlow * 10));
                    m_list.Add((ushort)drange.percentage);
                }
            }
            else
            {
                m_list.Add(0);
            }
            if (freqs != null)
            {
                m_list.Add((ushort)(GetFreqCount(freqs)));
                for (int i = 0; i < freqs.Length; i++)
                {
                    m_list.Add((ushort)(freqs[i] == true ? 1 : 0));
                }
            }
            else
            {
                m_list.Add(0);
            }
        }

        public void SendDataToController()
        {
            m_con.SendDataToController(m_list);
        }

        public void AddAlgo(ALGO_TYPES algoType,
                            EqualEnergyParams par,
                            bool[] freqs,
                            ushort drCycleTime,
                            ushort drPower)
        {

            m_list.Add((ushort)algoType);
            m_list.Add(drCycleTime);
            m_list.Add(drPower);

            m_list.Add(par.TotalSeconds);
            m_list.Add(par.agc);
            m_list.Add(par.mode);
            m_list.Add(par.highvalue);
            m_list.Add(par.lowvalue);
            m_list.Add(par.maxpower);
            m_list.Add(par.acckj);
            m_list.Add(par.targetkj);
            m_list.Add(par.absorbed);
            m_list.Add(par.singlerepetition);
            m_list.Add(par.toppercentage);
            // Set the count of how many dr range we have , we could have zero
            m_list.Add(0);
            if (freqs != null)
            {
                m_list.Add((ushort)(GetFreqCount(freqs)));
                for (int i = 0; i < freqs.Length; i++)
                {
                    m_list.Add((ushort)(freqs[i] == true ? 1 : 0));
                }
            }
            else
            {
                m_list.Add(0);
            }
        }
        public void Close()
        {
            m_tryConnect = false;
            m_connected = false;
            if (m_cookingResponseThread != null)
                m_cookingResponseThread.Join();

            m_con.Close();
        }

        public void AddAlgo(ALGO_TYPES algoType,
                           DRThresholdParams par,
                           List<DRRange> l,
                           bool[] freqs,
                           ushort drCycleTime, 
                           ushort drPower)
        {

            m_list.Add((ushort)algoType);

            m_list.Add(drCycleTime);
            m_list.Add(drPower);

            m_list.Add(par.TotalSeconds);
            m_list.Add(par.agc);


            // Set the count of how many dr range we have , we could have zero
            if (l != null)
            {
                m_list.Add((ushort)l.Count);
                foreach (DRRange drange in l)
                {
                    m_list.Add((ushort)(drange.drhigh * 10));
                    m_list.Add((ushort)(drange.drlow * 10));
                    m_list.Add((ushort)drange.percentage);
                }
            }
            else
            {
                m_list.Add(0);
            }
            if (freqs != null)
            {
                m_list.Add((ushort)(GetFreqCount(freqs)));
                for (int i = 0; i < freqs.Length; i++)
                {
                    m_list.Add((ushort)(freqs[i] == true ? 1 : 0));
                }
            }
            else
            {
                m_list.Add(0);
            }
        }
        
        public void AddAlgo(ALGO_TYPES algoType,
                           TimeSpan time)
                           
        {

            m_list.Add((ushort)algoType);
            m_list.Add((ushort)(time.TotalSeconds));
        }

        public void AddAlgo(ALGO_TYPES algoType,
                            ushort timeSeconds)
        {

            m_list.Add((ushort)algoType);
            m_list.Add((ushort)(timeSeconds));
        }

      
        public int GetFreqCount(bool [] freqs)
        {
            int count = 0;
            for (int i = 0 ; i < freqs.Length;i++)
            {
                if (freqs[i] == true)
                    count++;
            }
            if (count == freqs.Length)
                return 0;
            else
                return count;
        }
        public List<ushort> GetList()
        {
            return m_list;
        }

        public void AddAlgo(ALGO_TYPES algoType,
                           TopPercentageParams par,
                           bool[] freqs,
                           ushort drCycleTime, 
                           ushort drPower)
        {
            m_list.Add((ushort)algoType);

            m_list.Add(drCycleTime);
            m_list.Add(drPower);

            m_list.Add(par.TotalSeconds);
            m_list.Add(par.agc);
            m_list.Add(par.powertime);
            m_list.Add(par.maxpower);
            m_list.Add(par.toppercent);
            m_list.Add(par.targetkj);
            m_list.Add(par.absorbed);
            m_list.Add(par.equalEnergy);

            // Set the count of how many dr range we have , we could have zero
            m_list.Add(0);
            if (freqs != null)
            {
                m_list.Add((ushort)(GetFreqCount(freqs)));
                for (int i = 0; i < freqs.Length; i++)
                {
                    m_list.Add((ushort)(freqs[i] == true ? 1 : 0));
                }
            }
            else
            {
                m_list.Add(0);
            }
            
        }
        public void Clear()
        {
            m_list.Clear();
        }

        public void SetEqualEnergyDataCallback(EQNGValueCallbackMsg p)
        {

        }
        public void SetDRValuesCallback(DRValueCallbackMsg p)
        {
            m_DrCallback = p;
        }

        void TotalCookingTimeThread(TimeSpan totalRunningTime)
        {
            m_totalTimeEvent.Reset();
            bool signal = m_totalTimeEvent.WaitOne(totalRunningTime);
            if (signal == false)
                Stop();
        }
        void CookingResponseThread()
        {

            while (m_connected)
            {
                try
                {
                    StatusResponse r = m_con.ReadStatus();
                    if (r.running == 1)
                    {
                        wc(1, r.running.ToString());
                    }
                    else
                    {
                        wc(0, r.running.ToString());
                    }
                    if (r.drready == 1)
                    {
                        DR[] dr = m_con.GetDR();
                        if (m_DrCallback != null)
                            m_DrCallback(dr);
                    }
                    if (r.cycleEndded == 1)
                    {
                        wc(77, r.drCount.ToString());
                    }

                    wc(68, r.kj.ToString("0.000"));
                    string time = string.Format("{0},{1},{2}", r.clockSeconds.ToString("00"), r.clockMinutes.ToString("00"), r.clockHours.ToString("00"));
                    wc(90, time);
                    wc(95, r.AlgoClockSeconds.ToString());

                    if (m_lastRowIndex != r.rowIndex && r.running == 1)
                    {
                        wc(24, r.rowIndex.ToString());
                        m_lastRowIndex = r.rowIndex;
                    }
                    Thread.Sleep(1000);
                }
                catch (Exception err)
                {
                    if (m_mbedCommPortNumber)
                    {
                        wc(99, "Reset was performed: " + err.Message);
                        ResetController.Reset();

                    }
                    Thread.Sleep(1000);
                }
            }
        }
        public float[] GetAgcPower(byte forward)
        {
            return m_con.GetAgcPower(forward);
        }
        void StartTotalThread(TimeSpan totalRunningTime)
        {
            m_totalTimeEvent.Reset();
            if (m_totalCookingThread == null || m_totalCookingThread.IsAlive == false)
            {
                m_totalCookingThread = new Thread(() => TotalCookingTimeThread(totalRunningTime));
                m_totalCookingThread.Start();
            }
        }
    }
}
