using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopazCommonDefs;
using TopazControllerLib;

namespace TopazApi
{
    public sealed class SingleLog
    {
        string m_energyFileName;
        string m_DRFileName;
        string m_PowerInfoFileName;
        string m_KJPerFreqFileName;
        int m_testId;
        StreamWriter m_KJPerFreqWriter = null;
        StreamWriter m_energyWriter = null;
        StreamWriter m_powerInfoWriter = null;
        StreamWriter m_drWriter = null;
        Object m_lock_1 = new Object();
        Object m_lock_2 = new Object();
        bool m_useQueue = true;
        private static SingleLog instance=null;
        Queue<Tuple<DateTime, double>> m_energyQueue = new Queue<Tuple<DateTime, double>>();
        Queue<Tuple<DateTime, DR[]>> m_drQueue = new Queue<Tuple<DateTime, DR[]>>();

        Queue<Tuple<DateTime, EnergyData[]>> m_KJPerFreqQueue = new Queue<Tuple<DateTime, EnergyData[]>>();

        Queue<PowerInfo> m_powerQueue = new Queue<PowerInfo>();

        private static readonly object padlock = new object();


        private SingleLog()
        {

        }

        public static SingleLog Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SingleLog();
                    }
                    return instance;
                }
            }
        }
        public void WritePowerInfo(PowerInfo[] pin)
        {
            lock (m_lock_1)
            {
                try
                {
                    if (m_useQueue)
                    {
                        PowerInfo p = new PowerInfo();
                        foreach (PowerInfo  x  in pin)
                        {
                            if (x.valid)
                            {
                                p.mag_level = x.mag_level;
                                p.forward = x.forward;
                                p.reflected = x.reflected;
                                p.timestamp = x.timestamp;
                                p.valid = x.valid;
                                p.freq = x.freq;
                                p.watts = x.watts;
                                p.fwwatts = x.fwwatts;
                                p.rwwatts = x.rwwatts;
                                m_powerQueue.Enqueue(p);
                            }
                        }
                    }                    
                }
                catch (Exception err)
                {

                }
            }
        }
        private void DumpPowerInfoQueue()
        {
          
            int size = m_powerQueue.Count; 
            for (int i = 0; i <size; i++)
            {
                try
                {
                    PowerInfo t = m_powerQueue.Dequeue();
                    if (m_powerInfoWriter != null)
                        m_powerInfoWriter.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7}", t.timestamp, t.freq, t.forward, t.reflected, t.mag_level, t.watts , t.fwwatts, t.rwwatts);
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }
        private void DumpEnergyQueue()
        {
            int size = m_energyQueue.Count;
            for (int i = 0; i < size; i++)
            {
                try
                {
                    Tuple<DateTime, double> t = m_energyQueue.Dequeue();
                    if (m_energyWriter != null)
                        m_energyWriter.WriteLine("{0},{1}", t.Item1, t.Item2.ToString("0.00"));
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }
        public void WriteKJPerFreq(EnergyData[] KJPerFreq)
        {
            lock (m_lock_2)
            {
                try
                {
                    if (m_useQueue)
                    {
                        EnergyData[] d = new EnergyData[KJPerFreq.Length];
                        Array.Copy(KJPerFreq, d, KJPerFreq.Length);
                        m_KJPerFreqQueue.Enqueue(new Tuple<DateTime, EnergyData[]>(DateTime.Now, d));
                        return;
                    }

                    if (m_drWriter != null)
                    {
                        m_drWriter.WriteLine("+");
                    }

                    foreach (EnergyData d in KJPerFreq)
                    {
                        if (m_KJPerFreqWriter == null)
                            return;

                        m_KJPerFreqWriter.WriteLine("{0},{1},{2},{3},{4},{5}", DateTime.Now, d.freq, d.value);

                    }
                }
                catch (Exception err)
                {

                }
            }
        }
        public void WriteEnergy(double kj)
        {
            lock (m_lock_1)
            {
                try
                {

                    if (m_useQueue)
                    {
                        m_energyQueue.Enqueue(new Tuple<DateTime, double>(DateTime.Now, kj));
                    }
                    else
                        if (m_energyWriter != null)
                        {
                            m_energyWriter.WriteLine("{0},{1}", DateTime.Now, kj.ToString("0.00"));
                        }
                }
                catch (Exception err)
                {
                     
                }
            }
        }
        public void WriteDR(DR[] dr)
        {
            lock (m_lock_2)
            {
                try
                {
                    if (m_useQueue)
                    {
                        DR[] d = new DR[dr.Length];
                        Array.Copy(dr, d, dr.Length);
                        m_drQueue.Enqueue(new Tuple <DateTime, DR []>(DateTime.Now , d));
                        return;
                    }

                    if (m_drWriter != null)
                    {
                        m_drWriter.WriteLine("+");
                    }

                    foreach (DR d in dr)
                    {
                        if (d.valid == 1)
                        {
                            if (m_drWriter == null)
                                return;

                            m_drWriter.WriteLine("{0},{1},{2},{3},{4},{5}", DateTime.Now, d.freq, d.value);
                        }
                    }
                }
                catch (Exception err)
                {
                    
                }
            }
        }
              
        public void DumpDrQueue()
        {
            lock (m_lock_2)
            {
                try
                {
                    int size = m_drQueue.Count;
                    for (int i = 0; i < size; i++)
                    {
                        Tuple <DateTime, DR []> dr = m_drQueue.Dequeue();
                        if (m_drWriter != null)
                        {
                            m_drWriter.WriteLine("+");
                        }
                        foreach (DR d in dr.Item2)
                        {
                            if (m_drWriter == null)
                                return;

                            m_drWriter.WriteLine("{0},{1},{2},{3}", dr.Item1, d.freq, d.value, d.valid.ToString());
                        }
                    }
                }
                catch (Exception err)
                {
                   
                }
            }
        }
        public void DumpKjPerFreqQueue()
        {
            lock (m_lock_2)
            {
                try
                {
                    int size = m_KJPerFreqQueue.Count;
                    for (int i = 0; i < size; i++)
                    {
                        Tuple<DateTime, EnergyData[]> dr = m_KJPerFreqQueue.Dequeue();
                        if (m_KJPerFreqWriter != null)
                        {
                            m_KJPerFreqWriter.WriteLine("+");
                        }
                        foreach (EnergyData d in dr.Item2)
                        {
                            if (m_KJPerFreqWriter == null)
                                return;
                            if (d.value == 0)
                                 continue;
                            m_KJPerFreqWriter.WriteLine("{0},{1},{2}", dr.Item1, d.freq, d.value);
                        }
                    }
                }
                catch (Exception err)
                {

                }
            }
        }
        public void DumpTotalKjQueue(Queue<Tuple<DateTime, double>> q)
        {
            int size = q.Count;
            for (int i = 0 ; i < size; i++)
            {
                Tuple<DateTime, double> d = q.Dequeue();
                try
                {                 
                    m_energyWriter.WriteLine("{0},{1}", d.Item1, d.Item2.ToString("0.0000"));
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }
        public void FlushQueues()
        {
            lock (m_lock_1)
            {
                if (m_useQueue)
                {
                    DumpEnergyQueue();
                    DumpDrQueue();
                    DumpKjPerFreqQueue();
                    DumpPowerInfoQueue();
                }
            }
        }
        public void Close()
        {            
            lock (m_lock_1)
            {
                if (m_energyWriter != null)
                {
                    m_energyWriter.Flush();
                    m_energyWriter.Close();
                }
                m_energyWriter = null;

                 
                if (m_KJPerFreqWriter != null)
                {
                    m_KJPerFreqWriter.Flush();
                    m_KJPerFreqWriter.Close();
                }
                m_KJPerFreqWriter = null;


                if (m_powerInfoWriter != null)
                {
                    m_powerInfoWriter.Flush();
                    m_powerInfoWriter.Close();
                }
                m_powerInfoWriter = null;
                
            }

            lock (m_lock_2)
            {
                if (m_drWriter != null)
                    m_drWriter.Close();
                m_drWriter = null;
            }


            string energyFileName = DRIVE.Drive + @"TopazPOC\Energy\" + m_testId + "_energy.csv";
            string DRFileName = DRIVE.Drive + @"TopazPOC\DR\" + m_testId + "_dr.csv";
            string PowerInfoFileName = DRIVE.Drive + @"TopazPOC\Power\" + m_testId + "_dr.csv";
            string KJPerFreqFileName = DRIVE.Drive + @"TopazPOC\KJPerFreq\" + m_testId + "_kjpfreq.csv";

            if (DRIVE.Drive.ToLower()[0] != 'c')
            {
                if (m_energyFileName != null)
                    File.Copy(m_energyFileName, energyFileName);

                if (m_DRFileName != null)
                    File.Copy(m_DRFileName, DRFileName);
                if (m_PowerInfoFileName != null)
                    File.Copy(m_PowerInfoFileName, PowerInfoFileName);

                if (m_KJPerFreqFileName != null)
                    File.Copy(m_KJPerFreqFileName, KJPerFreqFileName);
            }
        }
        
        public void Create(int testId)
        {
            lock (m_lock_1)
            {
                m_testId = testId;
                try
                {
                    m_energyFileName = @"C:\Goji\TopazPOC\Energy\" + m_testId + "_energy.csv";
                    m_DRFileName = @"C:\Goji\TopazPOC\DR\" + m_testId + "_dr.csv";
                    m_PowerInfoFileName = @"C:\Goji\TopazPOC\Power\" + m_testId + "_dr.csv";
                    m_KJPerFreqFileName =  @"C:\Goji\TopazPOC\KJPerFreq\" + m_testId + "_kjpfreq.csv";

                    bool append = File.Exists(m_energyFileName);
                    if (append == true)
                    {
                        FileStream fs = new FileStream(m_energyFileName, FileMode.Append, FileAccess.Write);
                        m_energyWriter = new StreamWriter(fs);
                    }
                    else
                    {
                        FileStream fs = new FileStream(m_energyFileName, FileMode.CreateNew, FileAccess.Write);
                        m_energyWriter = new StreamWriter(fs);
                    }

                    append = File.Exists(m_KJPerFreqFileName);
                    if (append == true)
                    {
                        FileStream fs = new FileStream(m_KJPerFreqFileName, FileMode.Append, FileAccess.Write);
                        m_KJPerFreqWriter = new StreamWriter(fs);
                    }
                    else
                    {
                        FileStream fs = new FileStream(m_KJPerFreqFileName, FileMode.CreateNew, FileAccess.Write);
                        m_KJPerFreqWriter = new StreamWriter(fs);
                    }
                    
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }

            lock (m_lock_2)
            {
                try
                {
                    bool append = File.Exists(m_PowerInfoFileName);
                    if (append == true)
                    {
                        FileStream fs = new FileStream(m_PowerInfoFileName, FileMode.Append, FileAccess.Write);
                        m_powerInfoWriter = new StreamWriter(fs);
                    }
                    else
                    {
                        FileStream fs = new FileStream(m_PowerInfoFileName, FileMode.CreateNew, FileAccess.Write);
                        m_powerInfoWriter = new StreamWriter(fs);
                    }
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
             
            lock (m_lock_2)
            {
                try
                {
                    bool append = File.Exists(m_DRFileName);
                    if (append == true)
                    {
                        FileStream fs = new FileStream(m_DRFileName, FileMode.Append, FileAccess.Write);
                        m_drWriter = new StreamWriter(fs);
                    }
                    else
                    {
                        FileStream fs = new FileStream(m_DRFileName, FileMode.CreateNew, FileAccess.Write);
                        m_drWriter = new StreamWriter(fs);
                    }
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }

        
    }
}
