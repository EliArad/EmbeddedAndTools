using Newtonsoft.Json.Linq;
using RestApi;
using RestApi.HttpUtils;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TopazApiDefsCommon;
using TopazCommonDefs;

namespace TopazControllerLib
{
    
    public enum FLASH_LOCATION
    {
        AB_FORWARD = 10,
        AB_REFLECTED = 8
    }
    struct Response
    {
        public byte res;
        public byte reserved;
        public ushort opcode;
    };

    public struct FlashHeader
    {
        public int location;
        public int size;
    }
    public struct StatusResponse
    {
        public float kj;
        public byte clockSeconds;
        public byte clockMinutes;
        public byte clockHours;
        public byte cycleElapsedTime;
        public ushort AlgoClockSeconds;
        public byte running;
        public byte algoIndex;            
        public ushort opcode;
        public byte rowIndex;
        public byte drready;
        public byte cycleEndded;
        public byte drCount;
        public uint error1;
        public uint error2;
        public uint error3;                   
    };
     

    public struct ADCResponse
    {
        public ushort forward;
        public ushort reflected;
        public ushort opcode;
    };

    public abstract class ITopazControllerApi
    {
        protected DR[] m_userDR = new DR[MAX_FREQUENCIES];
        
        protected RestClient m_restClient = new RestClient();

        protected float[] m_power = new float[MAX_FREQUENCIES];
        protected ushort[] m_upower = new ushort[MAX_FREQUENCIES];

        public static readonly int MAX_FREQUENCIES = 101;

        protected static readonly object m_lock = new object();

        public abstract void BurnABFile(string fileName, DETECTOR det);
        public abstract void connect(string comPort, int BaudRate, bool autoDetect = false);
        public abstract void Close();
        public abstract void setMagnitude(ushort mag);
        public abstract void setFrequency(double frequency);
        public abstract TopazHwInfo ReadTopazHwInfo();
        public abstract void WriteTopazHwInfo(TopazHwInfo info);
        public abstract void AddAlgo(ushort[] list, bool start);
        public abstract void SendDataToController(List<ushort> list);
        public abstract StatusResponse ReadStatus();
        public abstract void ReadCalibration();
        public abstract void readADC(out ushort forward, out ushort reflected);

        public abstract DR [] GetDR();

        public abstract float[] GetAgcPower(byte forward);
        
        public abstract void Stop();
        public abstract void Start(ushort totalSeconds, bool oneTime);
        public abstract void StartDish(ushort dishId);
        public abstract void Pause(bool pause);

        protected int m_transferSize = 256;
        protected string m_url;
        protected int m_port;

        private SerialPort USB_PORT = new SerialPort();
 

        public void SetHttpUrl(string url, int port)
        {
            m_url = url;
            m_port = port;
            if (port > 0)
                m_url += ":" + port;
        }

        protected string GetUSB_ISS_PortName()
        {

            using (var searcher = new ManagementObjectSearcher
               ("SELECT * FROM WIN32_SerialPort"))
            {
                string[] portnames = SerialPort.GetPortNames();
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList();
                var tList = (from n in portnames
                             join p in ports on n equals p["DeviceID"].ToString()
                             select n + " - " + p["Caption"]).ToList();
                int i = 0;
                foreach (string s in tList)
                {
                    Console.WriteLine(s);
                    if (s.Contains(" - Communications Port (") == true)
                    {
                        return portnames[i];
                    }
                    i++;
                }
            }
            return string.Empty;
        }

        public virtual void TestPost()
        {
            lock (m_lock)
            {
                try
                {
                   
                    //string parameters = "?x=" + "4&y=2";
                    //m_restClient.EndPoint = m_url + "/Mult";

                    string parameters = "?x=" + "4eeeeeeeee";
                    m_restClient.EndPoint = m_url + "/Mult2";

                    m_restClient.Method = HttpVerb.POST;
                    m_restClient.PostData = null;
                    var json = m_restClient.MakeRequest(parameters);
                    dynamic d = JObject.Parse(json);
                    if (d.Result != "ok")
                    {
                        throw (new SystemException("failed to connect"));
                    }
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }

        public virtual string SendBreak(string ComPort)
        {

            try
            {
                USB_PORT.Close(); // close any existing handle
                USB_PORT.BaudRate = 115200;
                USB_PORT.PortName = ComPort;
                USB_PORT.StopBits = StopBits.One;
                USB_PORT.ReadTimeout = 4000;
                USB_PORT.WriteTimeout = 4000;
                USB_PORT.Open();
                if (USB_PORT.IsOpen == true)
                {
                    USB_PORT.BreakState = true;                    
                }
                USB_PORT.Close();
                return "ok";
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

    }
}
