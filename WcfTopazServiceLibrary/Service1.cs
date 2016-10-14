using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using TopazControllerLib;

namespace WcfTopazServiceLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
     ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class Service1 : IService1
    {
        DiolanController m_con = new DiolanController();
        public Service1()
        {
            try
            {
                ResetController.Reset();
                Console.WriteLine("Reset controller ok");
            }
            catch (Exception err)
            {

            }
        }
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }


        public Stream setFrequency(ushort freq)
        {
            try
            {
                m_con.setFrequency(freq);

                return PrepareResponseOk();

            }
            catch (Exception err)
            {
                return PrepareResponseMsg(err.Message);
            }
        }

        public Stream connect()
        {
            try
            {
                m_con.connect(string.Empty, 0);

                return PrepareResponseOk();

            }
            catch (Exception err)
            {
                dynamic jsonObject = new JObject();
                jsonObject.Result = err.Message;
                return jsonObject;
            }
        }

        Stream PrepareResponse(JObject jsonObject)
        {
            var s = JsonSerializer.Create();
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                s.Serialize(sw, jsonObject);
            }


            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
            return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
        }

        Stream PrepareResponseOk()
        {
            dynamic jsonObject = new JObject();
            jsonObject.Result = "ok";

            var s = JsonSerializer.Create();
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                s.Serialize(sw, jsonObject);
            }


            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
            return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
        }

        Stream PrepareResponseMsg(string msg)
        {
            dynamic jsonObject = new JObject();
            jsonObject.Result = msg;

            var s = JsonSerializer.Create();
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                s.Serialize(sw, jsonObject);
            }


            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
            return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
        }


        public Stream Close()
        {
            try
            {
                m_con.Close();
                return PrepareResponseOk();
            }
            catch (Exception err)
            {
                return PrepareResponseMsg(err.Message);
            }
        }

        public Stream setMagnitude(ushort mag)
        {
            try
            {
                m_con.setMagnitude(mag);
                return PrepareResponseOk();
            }
            catch (Exception err)
            {
                return PrepareResponseMsg(err.Message);
            }
        }


        public Stream SendDataToController(string data)
        {
            try
            {
                List<ushort> list = data.Split(',').Select(ushort.Parse).ToList();
                m_con.SendDataToController(list);
                return PrepareResponseOk();
            }
            catch (Exception err)
            {
                return PrepareResponseMsg(err.Message);
            }
        }

        public Stream ReadStatus()
        {
            StatusResponse  status = m_con.ReadStatus();

            dynamic jsonObject = new JObject();
            jsonObject.kj = status.kj;
            jsonObject.clockSeconds = status.clockSeconds;
            jsonObject.clockMinutes = status.clockMinutes;
            jsonObject.clockHours = status.clockHours;
            jsonObject.cycleElapsedTime = status.cycleElapsedTime;
            jsonObject.AlgoClockSeconds = status.AlgoClockSeconds;
            jsonObject.running = status.running;
            jsonObject.algoIndex = status.algoIndex;
            jsonObject.opcode = status.opcode;
            jsonObject.rowIndex = status.rowIndex;
            jsonObject.drready = status.drready;
            jsonObject.cycleEndded = status.cycleEndded;
            jsonObject.drCount = status.drCount;
            jsonObject.error1 = status.error1;
            jsonObject.error2 = status.error2;
            jsonObject.error3 = status.error3;
            
            var s = JsonSerializer.Create();
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                s.Serialize(sw, jsonObject);
            }
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
            return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
        }

        public Stream ReadCalibration()
        {
            try
            {
                 
                return PrepareResponseOk();
            }
            catch (Exception err)
            {
                return PrepareResponseMsg(err.Message);
            }
        }

        public Stream readADC()
        {
            try
            {
                 
                return PrepareResponseOk();
            }
            catch (Exception err)
            {
                return PrepareResponseMsg(err.Message);
            }
        }

        public long Multiply(long x, long y)
        {
            Console.WriteLine(x + y);
            return x * y;
        }

        public Stream Multiply2(string x)
        {
            Console.WriteLine(x);
            return PrepareResponseOk();
        }

        public Stream Stop()
        {
            try
            {
                m_con.Stop();
                return PrepareResponseOk();
            }
            catch (Exception err)
            {
                return PrepareResponseMsg(err.Message);
            }
        }

        public Stream Start(ushort totalSeconds, bool oneTime)
        {
            try
            {
                m_con.Start(totalSeconds , true);
                return PrepareResponseOk();
            }
            catch (Exception err)
            {
                return PrepareResponseMsg(err.Message);
            }
        }

        public Stream StartDish(ushort dishId)
        {
            try
            {
                m_con.StartDish(dishId);
                return PrepareResponseOk();
            }
            catch (Exception err)
            {
                return PrepareResponseMsg(err.Message);
            }
        }

        public Stream SendBreak(string ComPort)
        {
            try
            {
                m_con.SendBreak(ComPort);
                return PrepareResponseOk();
            }
            catch (Exception err)
            {
                return PrepareResponseMsg(err.Message);
            }
        }


        public Stream Reset()
        {
            try
            {
                ResetController.Reset();
                return PrepareResponseOk();
            }
            catch (Exception err)
            {
                return PrepareResponseMsg(err.Message);
            }
        }

        public Stream Pause(bool pause)
        {
            try
            {
                m_con.Pause(pause);
                return PrepareResponseOk();
            }
            catch (Exception err)
            {
                return PrepareResponseMsg(err.Message);
            }
        }
    }
}
