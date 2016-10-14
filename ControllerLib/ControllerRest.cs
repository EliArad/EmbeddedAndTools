using Newtonsoft.Json.Linq;
using RestApi;
using RestApi.HttpUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopazCommonDefs;

namespace TopazControllerLib
{
    public class TopazRestApi : ITopazControllerApi
    {

        StatusResponse m_statusResponse = new StatusResponse();

        public override void setFrequency(double frequency)
        {
            lock (m_lock)
            {
                try
                {
                    string parameters = "?freq=" + frequency;
                    m_restClient.EndPoint = m_url + "/setFrequency";
                    m_restClient.Method = HttpVerb.GET;
                    m_restClient.PostData = null;
                    var json = m_restClient.MakeRequest(parameters);
                    dynamic d = JObject.Parse(json);
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }
        public override void readADC(out ushort forward, out ushort reflected)
        {
            lock (m_lock)
            {
                try
                {
                    forward = 0; reflected = 0;
                    m_restClient.EndPoint = m_url + "/readADC";
                    m_restClient.Method = HttpVerb.GET;
                    m_restClient.PostData = null;
                    var json = m_restClient.MakeRequest();
                    dynamic d = JObject.Parse(json);
                    string k = d.forward;
                    forward = ushort.Parse(k);
                    k = d.reflected;
                    reflected = ushort.Parse(k);
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }
        public override void setMagnitude(ushort mag)
        {
            lock (m_lock)
            {
                try
                {

                    string parameters = "?mag=" + mag;
                    m_restClient.EndPoint = m_url + "/setMagnitude";
                    m_restClient.Method = HttpVerb.GET;
                    m_restClient.PostData = null;
                    var json = m_restClient.MakeRequest(parameters);
                    dynamic d = JObject.Parse(json);
                    if (d.Result != "ok")
                    {
                        throw (new SystemException("Error"));
                    }
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }

        public override void BurnABFile(string fileName, TopazCommonDefs.DETECTOR det)
        {
            throw new NotImplementedException();
        }

        public override void connect(string comPort, int BaudRate, bool autoDetect = false)
        {
            lock (m_lock)
            {
                try
                {
                    m_restClient.EndPoint = m_url + "/connect";
                    m_restClient.Method = HttpVerb.GET;
                    m_restClient.PostData = null;
                    var json = m_restClient.MakeRequest();
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

        public override string SendBreak(string ComPort)
        {
          

            lock (m_lock)
            {
                try
                {
                    string parameters = "?ComPort=" + ComPort;
                    m_restClient.EndPoint = m_url + "/SendBreak";
                    m_restClient.Method = HttpVerb.GET;
                    m_restClient.PostData = null;
                    var json = m_restClient.MakeRequest(parameters);
                    dynamic d = JObject.Parse(json);
                    if (d.Result != "ok")
                    {
                        throw (new SystemException("failed to connect"));
                    }
                    return "ok";
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override TopazApiDefsCommon.TopazHwInfo ReadTopazHwInfo()
        {
            throw new NotImplementedException();
        }
        public override DR[] GetDR()
        {
            throw new NotImplementedException();
        }
        public override void WriteTopazHwInfo(TopazApiDefsCommon.TopazHwInfo info)
        {
            throw new NotImplementedException();
        }
        public override void SendDataToController(List<ushort> list)
        {
             

            lock (m_lock)
            {
                try
                {

                    ushort[] data = list.ToArray();
                    string str = String.Join(",", list);
                    string parameters = "?data=" + str;
                    m_restClient.EndPoint = m_url + "/SendDataToController";
                    m_restClient.Method = HttpVerb.GET;
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
        public override void AddAlgo(ushort[] list, bool start)
        {
            throw new NotImplementedException();
        }

        public override StatusResponse ReadStatus()
        {

            lock (m_lock)
            {
                try
                {

                    m_restClient.EndPoint = m_url + "/ReadStatus";
                    m_restClient.Method = HttpVerb.GET;
                    m_restClient.PostData = null;
                    var json = m_restClient.MakeRequest();
                    dynamic d = JObject.Parse(json);
                    if (d.opcode != OPCODES.OPC_READ_STATUS)
                    {
                        throw (new SystemException("failed to connect"));
                    }

                    m_statusResponse.kj = d.kj;
                    m_statusResponse.clockSeconds = d.clockSeconds;
                    m_statusResponse.clockMinutes = d.clockMinutes;
                    m_statusResponse.clockHours = d.clockHours;
                    m_statusResponse.cycleElapsedTime = d.cycleElapsedTime;
                    m_statusResponse.AlgoClockSeconds = d.AlgoClockSeconds;
                    m_statusResponse.running = d.running;
                    m_statusResponse.algoIndex = d.algoIndex;
                    m_statusResponse.opcode = d.opcode;
                    m_statusResponse.rowIndex = d.rowIndex;
                    m_statusResponse.drready = d.drready;
                    m_statusResponse.cycleEndded = d.cycleEndded;
                    m_statusResponse.drCount = d.drCount;
                    m_statusResponse.error1 = d.error1;
                    m_statusResponse.error2 = d.error2;
                    m_statusResponse.error3 = d.error3;

                    return m_statusResponse;
                    
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }
        public override void StartDish(ushort dishId)
        {
            throw new NotImplementedException();
        }
        public override void Start(ushort totalSeconds, bool oneTime)
        {
            lock (m_lock)
            {
                try
                {
                    string parameters = "?totalSeconds=" + totalSeconds + "&oneTime=" + oneTime;
                    m_restClient.EndPoint = m_url + "/Start";
                    m_restClient.Method = HttpVerb.GET;
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
        public override void ReadCalibration()
        {
            throw new NotImplementedException();
        }
        public override void Stop()
        {
            lock (m_lock)
            {
                try
                {

                    m_restClient.EndPoint = m_url + "/Stop";
                    m_restClient.Method = HttpVerb.GET;
                    m_restClient.PostData = null;
                    var json = m_restClient.MakeRequest();
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

        public override float[] GetAgcPower(byte forward)
        {
            throw new NotImplementedException();
        }

        public override void Pause(bool pause)
        {
            throw new NotImplementedException();
        }
    }
}
