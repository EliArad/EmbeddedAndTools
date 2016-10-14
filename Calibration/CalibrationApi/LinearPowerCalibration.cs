using MathUtilLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TopazCommonDefs;

namespace TopazCalibrationApi
{
    public class LinearPowerCalibration : TopazCalibration
    {
        Thread m_thread = null;
        DETECTOR m_detectors;
        string m_fileName;
        string m_fileCName;
        ushort m_startCode, m_endCode, m_stepCode;

        Dictionary<double, List<double>> X = new Dictionary<double, List<double>>();
        Dictionary<double, List<double>> Y = new Dictionary<double, List<double>>();

        public LinearPowerCalibration(MessageCallback p) : base(p)
        {
            for (float freq = 2400; freq <= 2500; freq++)
            {
                X.Add(freq, new List<double>());
                Y.Add(freq, new List<double>());
            }
        }

        void Calibrate()
        {
            m_running = true;
            ushort scode;
            double power;
            pCallback(1, "Started");
                        
            for (scode = m_startCode; scode < m_endCode; scode+=m_stepCode)
            {
                if (m_running == false)
                {
                    m_con.setMagnitude(0);
                    return;
                }
                m_con.setMagnitude(scode);
                Thread.Sleep(10);
                for (float freq = 2400; freq <= 2500; freq++)
                {
                    if (m_running == false)
                    {
                        m_con.setMagnitude(0);
                        return;
                    }
                    m_con.setFrequency(freq);
                    Thread.Sleep(1);
                    double cp = m_coupler.getPower2(freq, m_detectors == DETECTOR.FORWARD ? CouplerLib.PORT_TYPE.FORWARD: CouplerLib.PORT_TYPE.REFLECTED );
                    power = m_powerMeter[(int)m_detectors].Read(10) + cp;
                    X[freq].Add(scode);
                    Y[freq].Add(power);
                    pCallback(40 , string.Format("{0},{1},{2}",freq,scode,power.ToString("0.000")));
                    Thread.Sleep(1);
                }
            }            
            CalcABLinear();
            m_running = false;
            m_con.setMagnitude(0);
            pCallback(2, "finished");
        }
        void BurnABFile(string fileName, DETECTOR det)
        {
            m_con.BurnABFile(fileName, det);
        }
        void CalcABLinear()
        {
            try
            {
                StreamWriter sw = new StreamWriter(m_fileName);
                StreamWriter sw1 = new StreamWriter(m_fileCName);
                for (float freq = 2400; freq <= 2500; freq++)
                {
                    Dictionary<string, double> result = MathUtil.SimpleLR(X[freq], Y[freq]);
                    sw.WriteLine(result["Beta"].ToString() + ","  + result["Alpha"].ToString());

                    double a = result["Beta"];
                    double b = result["Alpha"];
                    a = a * 1000000000;
                    a = a * 100;
                    b = b * 100000;
                    sw1.WriteLine(a.ToString() + "," + b.ToString());
                }
                sw.Close();
                sw1.Close();
            }
            catch (Exception err)
            {
                pCallback(100, err.Message);
            }
        }
        public void Configure(DETECTOR det, ushort startCode, ushort endCode, ushort stepCode)
        {
            m_detectors = det;
            m_startCode = startCode;
            m_endCode = endCode;
            m_stepCode = stepCode;
            if (det == DETECTOR.FORWARD)
            {
                m_fileName = "ab_forward.csv";
                m_fileCName = "ab_forward_c.csv";
            }
            else
            {
                m_fileName = "ab_reflected.csv";
                m_fileCName = "ab_reflected_c.csv";
            }
        }
        public void Start()
        {
            if (m_connected == false)
            {
                throw (new SystemException("Controller not connected"));
            }
            if (m_thread == null || m_thread.IsAlive == false)
            {
                m_thread = new Thread(Calibrate);
                m_thread.Start();
            }
            else
            {
                throw (new SystemException("Already started"));
            }
        }
        public void Stop()
        {
            m_running = false;
            if (m_thread != null)
                m_thread.Join();
        }
    }
}
