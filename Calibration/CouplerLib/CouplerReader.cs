using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CouplerLib
{
    public enum COUPLER_SERIAL
    {
        LIOR_COUPLER_01,
        GREEN_99832,
        NONE
    }
    public enum  PORT_TYPE
    {
        FORWARD,
        REFLECTED,
        FORWARD_WITH_CIRCULATOR
    }
    public class CouplerReader
    {
        protected string m_path;
        protected string m_couplerForwardFile;
        protected string m_couplerReflectedFile;
        protected string m_couplerForwardNoCirculatorFile;

        double[,] Data = new double[3,101];
        Dictionary<double, double>[] m_dicData1 = new Dictionary<double, double>[3];
        Dictionary<double, double>[] m_dicData2 = new Dictionary<double, double>[3];
        Dictionary<double, double>[] m_dicData3 = new Dictionary<double, double>[3];
        

        public CouplerReader(COUPLER_SERIAL serial)
        {
            switch (serial)
            {
                case COUPLER_SERIAL.LIOR_COUPLER_01:
                    m_path = @"C:\Goji\Coupler\LiorSerial001\";
                    m_couplerForwardFile = "FORWARD_with_ circulator.CSV";
                    m_couplerReflectedFile = "REFLECTED.CSV";
                    m_couplerForwardNoCirculatorFile = @"FORWARD_with_ circulator.CSV";
                break;

                case COUPLER_SERIAL.GREEN_99832:
                    m_path = @"C:\Goji\Coupler\99832\";
                    m_couplerForwardFile = "FORWARD.CSV";
                    m_couplerReflectedFile = "REFLECTED.CSV";
                    m_couplerForwardNoCirculatorFile = @"";
                break;

                    
            }                               
            try
            {

                m_dicData1[0] = new Dictionary<double, double>();
                m_dicData1[1] = new Dictionary<double, double>();
                m_dicData1[2] = new Dictionary<double, double>();

                m_dicData2[0] = new Dictionary<double, double>();
                m_dicData2[1] = new Dictionary<double, double>();
                m_dicData2[2] = new Dictionary<double, double>();


                m_dicData3[0] = new Dictionary<double, double>();
                m_dicData3[1] = new Dictionary<double, double>();
                m_dicData3[2] = new Dictionary<double, double>();

                Load(serial);
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
        public double getPower1(double freq, PORT_TYPE port)
        {
            try
            {
                string t = freq.ToString("0.000");
                freq = double.Parse(t);
                double value = m_dicData1[(int)port][freq];
                return value;
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
        public double getPower2(double freq, PORT_TYPE port)
        {
            try
            {
                double value = m_dicData2[(int)port][freq];
                return value;
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
        public double getPower3(double freq, PORT_TYPE port)
        {
            try
            {
                double value = m_dicData3[(int)port][freq];
                return value;
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
        void Load(COUPLER_SERIAL serial)
        {
            string line;
            string[] fileNames =  {
           
                m_path + m_couplerForwardFile,
                m_path + m_couplerReflectedFile,
                m_path + m_couplerForwardNoCirculatorFile
            };
            int n = 0;
            foreach(string fname in fileNames)
            {
                if (File.Exists(fname) == false)
                    continue;
                using (StreamReader sr = new StreamReader(fname))
                {
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    int i = 0;
                    while (true)
                    {
                        line = sr.ReadLine();
                        if (line == null)
                            break;
                        string[] s = line.Split(new Char[] { ',' });
                        double freq = double.Parse(s[0]);
                        double value = double.Parse(s[1]);
                        if (value < 0)
                            value *= -1;
                        Data[n, i] = value;
                        
                        m_dicData1[n].Add(freq / 1E9, value);
                        m_dicData2[n].Add(freq / 1E6, value);
                        m_dicData3[n].Add(freq, value);
                        i++;
                    }
                }
                n++;
            }
        }
    }    
}
