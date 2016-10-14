using Dln;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TopazApiDefsCommon;
using TopazCommonDefs;
 

namespace TopazControllerLib
{
    public class DiolanController : ITopazControllerApi
    {

        const byte sla = 0x7F;
        //const byte m_write_i2c_address_shift = (0x7F << 1);
        //const byte m_read_i2c_address_shift = (byte)((0x7F << 1) + 1);

        Device device;
        Dln.I2cMaster.Port i2c;
 
        
        bool m_saveAsText = false;
        public DiolanController()
        {
            
            int i = 0;
            for (float f = 2400; f<= 2500; f++)
            {
                m_userDR[i].freq = f;
                i++;
            }
        }
        
         
        public override void connect(string comPort, int BaudRate, bool autoDetect = false)
        {
            try
            {

                // Connect to DLN server
                Library.Connect("localhost", Connection.DefaultPort);

                // Open device
                if (Device.Count() == 0)
                {
                    throw(new SystemException("No DLN-series adapters have been detected."));
                }
                device = Device.Open();

                int portCount = device.I2cMaster.Ports.Count;
                if (portCount == 0)
                {
                    throw(new SystemException("Current DLN-series adapter doesn't support I2C Master interface."));
                }
                i2c = device.I2cMaster.Ports[0];

                
                Console.WriteLine("Frequency: " + i2c.Frequency);
                i2c.Enabled = true;
               
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

         
        byte[] getBytes<T>(T str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        protected void ByteArrayToStruct<T>(byte[] packet, ref T str)
        {
            GCHandle pinnedPacket = GCHandle.Alloc(packet, GCHandleType.Pinned);
            str = (T)Marshal.PtrToStructure(
                pinnedPacket.AddrOfPinnedObject(),
                typeof(T));
            pinnedPacket.Free();
        }

        public override void BurnABFile(string fileName, DETECTOR det)
        {
            if (m_saveAsText == true)
            {
                byte[] fileBytes = File.ReadAllBytes(fileName);
                WriteToFlash(fileBytes, det == DETECTOR.FORWARD ? FLASH_LOCATION.AB_FORWARD : FLASH_LOCATION.AB_REFLECTED);
            }
            else
            {
                byte[] buffer = new byte[16 * 101];
                using (StreamReader sr = new StreamReader(fileName))
                {
                    int index = 0;
                    while (true)
                    {
                        string line = sr.ReadLine();
                        if (line == null)
                            break;
                        string[] sdata = line.Split(new Char[] { ',' });
                        double a = double.Parse(sdata[0]);
                        byte[] aArray = BitConverter.GetBytes(a);
                        Array.Copy(aArray, 0, buffer, index, aArray.Length);
                        index += aArray.Length;
                        double b = double.Parse(sdata[1]);
                        byte[] bArray = BitConverter.GetBytes(b);
                        Array.Copy(bArray, 0, buffer, index, bArray.Length);
                        index += bArray.Length;
                    }
                    WriteToFlash(buffer, det == DETECTOR.FORWARD ? FLASH_LOCATION.AB_FORWARD : FLASH_LOCATION.AB_REFLECTED);
                }
            }
        }
        void WriteToFlash(byte[] data, FLASH_LOCATION loc)
        {
            byte[] buffer = new byte[8];
            
            buffer[3] = (byte)((int)loc >> 24);
            buffer[2] = (byte)((int)loc >> 16);
            buffer[1] = (byte)((int)loc >> 8);
            buffer[0] = (byte)(int)loc;

            int size = data.Length;

            buffer[7] = (byte)(size >> 24);
            buffer[6] = (byte)(size >> 16);
            buffer[5] = (byte)(size >> 8);
            buffer[4] = (byte)size;

            WriteFlashPhraseChunk(buffer);

            size = data.Length;
            byte [] toSend = new byte[8];
            int index = 0;
            while (size > 0)
            {
                int x = Math.Min(size, 8);
                Array.Clear(toSend, 0, 8);
                Array.Copy(data, index, toSend, 0, x);
                //for (int i = 0; i < 8; i++)
                  //  Console.Write(toSend[i].ToString("X"));
                //Console.WriteLine();
                WriteFlashPhraseChunk(toSend);
                index += x;
                size -= x;
            }
        }
        void WriteFlashPhraseChunk(byte [] data)
        {
            lock (m_lock)
            {
                try
                {
                    
                    List<byte> SerBuf = new List<byte>();
                    AddPrehumble(ref SerBuf);
                    AddOpcode(ref SerBuf, OPCODES.OPC_WRITE_FLASH_CHUNK);
                    ushort size = (ushort)(data.Length);
                    AddSize(ref SerBuf, size);

                    for (int j = 0; j < size; j++)
                    {
                        SerBuf.Add(data[j]);
                    }
                    i2c.Write(sla,SerBuf.ToArray());
                   
                    Thread.Sleep(1); // because of the response we need to wait 
                    readCommandResponse(OPCODES.OPC_WRITE_FLASH_CHUNK);
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }
        public override void SendDataToController(List<ushort> list)
        {
            list.Add(0xF1F2); // we mark the end of the list for the controller to east detect it
            list.Insert(0, (ushort)list.Count);
            ushort[] data = list.ToArray();
            AddAlgo(data, true);
        }
        void AddShort(ref List<byte> SerBuf, ushort data)
        {
            SerBuf.Add((byte)(data & 0xFF));
            SerBuf.Add((byte)((data >> 8) & 0xFF));
        }
        public override void Start(ushort totalSeconds, bool oneTime)
        {
            lock (m_lock)
            {
                try
                {
                    List<byte> SerBuf = new List<byte>();

                    AddPrehumble(ref SerBuf);
                    AddOpcode(ref SerBuf, OPCODES.OPC_START_COOKING);
                    AddSize(ref SerBuf, 3);
                    AddShort(ref SerBuf, totalSeconds);
                    SerBuf.Add((byte)(oneTime == true ? 1 : 0));
                    i2c.Write(sla, SerBuf.ToArray());

                    Thread.Sleep(1); // because of the response we need to wait 
                    readCommandResponse(OPCODES.OPC_START_COOKING);
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }

        public override void Stop()
        {
            lock (m_lock)
            {
                try
                {

                    List<byte> SerBuf = new List<byte>();

                    AddPrehumble(ref SerBuf);
                    AddOpcode(ref SerBuf, OPCODES.OPC_STOP_COOKING);
                    AddSize(ref SerBuf, 0);
                    i2c.Write(sla, SerBuf.ToArray());

                    Thread.Sleep(1); // because of the response we need to wait 
                    readCommandResponse(OPCODES.OPC_STOP_COOKING);

                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }

        public override void StartDish(ushort dishId)
        {
            lock (m_lock)
            {
                try
                {

                    List<byte> SerBuf = new List<byte>();

                    AddPrehumble(ref SerBuf);
                    AddOpcode(ref SerBuf, OPCODES.OPC_START_DISH_COOKING);
                    AddSize(ref SerBuf, 2);
                    SerBuf.Add((byte)(dishId & 0xFF));
                    SerBuf.Add((byte)((dishId >> 8) & 0xFF));
                    i2c.Write(sla, SerBuf.ToArray());
                      
                    Thread.Sleep(1); // because of the response we need to wait 
                    readCommandResponse(OPCODES.OPC_SET_FREQUENCY);

                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }

        public override DR[] GetDR()
        {
            lock (m_lock)
            {
                try
                {

                    List<byte> SerBuf = new List<byte>();
                    AddPrehumble(ref SerBuf);

                    AddOpcode(ref SerBuf, OPCODES.OPC_GET_DR);
                    AddSize(ref SerBuf, 0);
                    i2c.Write(sla, SerBuf.ToArray());

                    Thread.Sleep(1); // because of the response we need to wait 

                    return readDRResponse();
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }
        DR[] readDRResponse()
        {
            byte [] drvalue = new byte[MAX_FREQUENCIES + 13];
            i2c.Read(sla, drvalue);
            
            for (int i = 0; i < MAX_FREQUENCIES; i++)
            {
                m_userDR[i].value = (float)(drvalue[i] / 10.0);
                m_userDR[i].valid = 1;
            }
            return m_userDR;
        }
        public override void Pause(bool pause)
        {
            lock (m_lock)
            {
                try
                {

                    List<byte> SerBuf = new List<byte>();
                    AddPrehumble(ref SerBuf);
                    
                    AddOpcode(ref SerBuf, OPCODES.OPC_PAUSE_COOKING);
                    AddSize(ref SerBuf, 2);

                    ushort x = (ushort)(pause == true ? 1 : 0);
                    SerBuf.Add( (byte)(x & 0xFF));
                    SerBuf.Add((byte)((x >> 8) & 0xFF));
                    i2c.Write(sla, SerBuf.ToArray());
                    Thread.Sleep(1); // because of the response we need to wait 
                    readCommandResponse(OPCODES.OPC_PAUSE_COOKING);
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }

        public override void setFrequency(double frequency)
        {
            lock (m_lock)
            {
                try
                {
                    
                    List<byte> SerBuf = new List<byte>();
                     
                    SerBuf.Add(0x52);
                    SerBuf.Add(0x91);                    
                    AddOpcode(ref SerBuf, OPCODES.OPC_SET_FREQUENCY);

                    ushort size = 2;
                    SerBuf.Add((byte)(size & 0xFF));
                    SerBuf.Add((byte)((size >> 8) & 0xFF));
                    ushort x = (ushort)((frequency - 2400) * 2);
                    SerBuf.Add((byte)(x & 0xFF));
                    SerBuf.Add((byte)((x >> 8) & 0xFF));
                     
                    i2c.Write(sla, SerBuf.ToArray());
                    
                    Thread.Sleep(1); // because of the response we need to wait 
                    readCommandResponse(OPCODES.OPC_SET_FREQUENCY);

                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }
          
        public override TopazHwInfo ReadTopazHwInfo()
        {
            lock (m_lock)
            {
                try
                {
                    List<byte> SerBuf = new List<byte>();
                    AddPrehumble(ref SerBuf);                   
                    AddOpcode(ref SerBuf, OPCODES.OPC_READ_TOPAZ_INFO);
                    AddSize(ref SerBuf, 0);
                    i2c.Write(sla, SerBuf.ToArray());
                    
                    Thread.Sleep(2); // because of the response we need to wait 
                    return readHwInfoResponse(OPCODES.OPC_READ_TOPAZ_INFO);
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }

        public override void WriteTopazHwInfo(TopazHwInfo info)
        {

            lock (m_lock)
            {
                try
                {
                    List<byte> SerBuf = new List<byte>();
                
                    byte[] b1 = System.Text.Encoding.UTF8.GetBytes (info.serialNumber);
                    if (b1.Length > 8)
                        throw (new SystemException("size is big"));
                    
                    byte[] b2 = System.Text.Encoding.UTF8.GetBytes (info.partNumber);
                    if (b2.Length > 8)
                        throw (new SystemException("size is big"));

                    
                    byte[] b1Data = new byte[8];
                    Array.Copy(b1, b1Data, b1.Length);
                    /*
                    byte[] b2Data = new byte[8];
                    Array.Copy(b2, b2Data, b2.Length);
                    */

                    ushort size = (ushort)(b1Data.Length);// +b2.Length;

                    AddPrehumble(ref SerBuf);                
                    AddOpcode(ref SerBuf, OPCODES.OPC_WRITE_TOPAZ_INFO);
                    AddSize(ref SerBuf, size); 
                     
                    // Data start here:
                    for (int j = 0; j < b1Data.Length; j++)
                        SerBuf.Add(b1Data[j]);

                    i2c.Write(sla, SerBuf.ToArray());
                    
                    Thread.Sleep(2); // because of the response we need to wait 
                    readCommandResponse(OPCODES.OPC_WRITE_TOPAZ_INFO);
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }
        void AddPrehumble(ref List<byte> SerBuf)
        {
            SerBuf.Add((byte)0x52);
            SerBuf.Add((byte)0x91);
        }
        public override void AddAlgo(ushort[] list, bool start)
        {
            lock (m_lock)
            {
                try
                {
                   
                    List<byte> SerBuf = new List<byte>();
                    AddPrehumble(ref SerBuf);
                   
                    OPCODES opcode;
                    if (start == true)
                    {
                        opcode = OPCODES.OPC_ADDALGO; 
                    }
                    else
                    {
                        opcode = OPCODES.OPC_APPENDALGO; 
                    }

                    AddOpcode(ref SerBuf, opcode);

                    ushort size = (ushort)(list.Count() * 2);
                    AddSize(ref SerBuf, size);
                    for (int j = 0; j < list.Length; j++)
                    {
                        SerBuf.Add((byte)(list[j] & 0xFF));
                        SerBuf.Add((byte)((list[j] >> 8) & 0xFF));
                    }
                    i2c.Write(sla, SerBuf.ToArray());
                    Thread.Sleep(2); // because of the response we need to wait 
                    readCommandResponse(opcode);
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }

        TopazHwInfo readHwInfoResponse(OPCODES opcode)
        {
            List<byte> SerBuf = new List<byte>();
             
           
            TopazHwInfo r = new TopazHwInfo();
                        
            return r;
        }

        void readCommandResponse(OPCODES opcode)
        {
            int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Response));
            byte[] Buffer = new byte[size];
            i2c.Read(sla, Buffer);
            Response r = new Response();
            ByteArrayToStruct(Buffer, ref r);
            if (r.opcode != (ushort)opcode || r.res != 1)
            {   
                throw (new SystemException("Incorrect response"));
            }
        }
        StatusResponse readStatusResponse()
        {
            int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(StatusResponse));

            byte[] buffer = new byte[size];
            i2c.Read(sla, buffer);                         
            StatusResponse r = new StatusResponse();
            ByteArrayToStruct(buffer, ref r);
            return r;
        }

        public ADCResponse readADCResponse()
        {
            try
            {
                byte[] buffer = new byte[6];

                i2c.Read(sla, 0, 0 , buffer, 6);
                ADCResponse r = new ADCResponse();
                ByteArrayToStruct(buffer, ref r);
                return r;
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }

        byte [] readData(int size)
        {
            byte[] buffer = new byte[size];
            i2c.Read(sla, 0, 0, buffer, size);
            return buffer;
        }
         
        public ushort getMagnitude()
        {

            return 0;
        }

        public ushort getFrequency()
        {

            return 0;
        }
 
       
        public override StatusResponse ReadStatus()
        {
            lock (m_lock)
            {
                try
                {

                    List<byte> SerBuf = new List<byte>();
                    AddPrehumble(ref SerBuf);
 
                    AddOpcode(ref SerBuf, OPCODES.OPC_READ_STATUS);

                    AddSize(ref SerBuf, 0);

                    i2c.Write(sla, SerBuf.ToArray());
                    
                    Thread.Sleep(10); // because of the response we need to wait 
                    StatusResponse r = readStatusResponse();

                    if (r.opcode != (ushort)OPCODES.OPC_READ_STATUS)
                    {
                        throw (new SystemException("Data is not valid"));
                    }
                    return r;
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }

        FlashHeader ReadFlashHeaderSize(FLASH_LOCATION location)
        {
            lock (m_lock)
            {
                try
                {
                    List<byte> SerBuf = new List<byte>();
                    AddPrehumble(ref SerBuf);
 
                    AddOpcode(ref SerBuf, OPCODES.OPC_READ_FLASH_CHUNK);

                    AddSize(ref SerBuf, 2);

                    SerBuf.Add((byte)((int)location & 0xFF));
                    SerBuf.Add((byte)(((int)location >> 8) & 0xFF));

                    i2c.Write(sla, SerBuf.ToArray());
                    Thread.Sleep(10);
                    byte[] r = readData(8);
                    FlashHeader fh = new FlashHeader();
                    ByteArrayToStruct(r, ref fh);
                    if ((FLASH_LOCATION)fh.location != location)
                    {
                        throw (new SystemException("Error invalid location"));
                    }
                    return fh;
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }
        public override void ReadCalibration()
        {

            lock (m_lock)
            {
                try
                {
                    string[] fileName = { "Data\\ab_forward_c.csv", "Data\\ab_reflected_c.csv" };
                    string[] _fileName = { "Data\\ab_forward.csv", "Data\\ab_reflected.csv" };
                    for (int n = 0; n < 2; n++)
                    {
                        FLASH_LOCATION fl = n == 0 ? FLASH_LOCATION.AB_FORWARD : FLASH_LOCATION.AB_REFLECTED;
                        FlashHeader header = ReadFlashHeaderSize(fl);
                        int CHUNK_SIZE = 8;
                        StreamWriter sw = new StreamWriter(fileName[n]);
                        StreamWriter sw1 = new StreamWriter(_fileName[n]);
                        for (int i = 0; i < 101; i++)
                        {                             
                            byte[] data = ReadChunk(CHUNK_SIZE);
                            double a = BitConverter.ToDouble(data, 0);
                            sw.Write("{0},", a);
                            a = a  / 1000000.0;
			                a = a / 100000;
                            sw1.Write("{0},", a);
                            data = ReadChunk(CHUNK_SIZE);
                            double b = BitConverter.ToDouble(data, 0);
                            sw.WriteLine("{0}", b);
                            b = b / 100000;
                            sw1.WriteLine("{0}", b);
                        }
                        sw.Close();
                        sw1.Close();
                    }
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }
        void AddOpcode(ref List<byte> SerBuf, OPCODES opcode)
        {
            SerBuf.Add((byte)((ushort)opcode & 0xFF));
            SerBuf.Add((byte)(((ushort)opcode >> 8) & 0xFF));
        }
        void AddSize(ref List<byte> SerBuf , ushort size)
        {
            SerBuf.Add((byte)(size & 0xFF));
            SerBuf.Add((byte)((size >> 8) & 0xFF));
        }
        byte [] ReadChunk(int chunkSize)
        {

            lock (m_lock)
            {
                try
                {
                     
                    List<byte> SerBuf = new List<byte>();
                    AddPrehumble(ref SerBuf);
                    AddOpcode(ref SerBuf, OPCODES.OPC_READ_FLASH_CHUNK);
                    AddSize(ref SerBuf, 2);
                    SerBuf.Add((byte)(chunkSize & 0xFF));
                    SerBuf.Add((byte)((chunkSize >> 8) & 0xFF));
                    i2c.Write(sla, SerBuf.ToArray());
 
                    Thread.Sleep(10);
                    byte[] r = readData(chunkSize);
                    return r;
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }

        public override float [] GetAgcPower(byte forward)
        {
            lock (m_lock)
            {
                try
                {

                    List<byte> SerBuf = new List<byte>();
                    AddPrehumble(ref SerBuf);
                    AddOpcode(ref SerBuf, OPCODES.OPC_GET_COOKING_DATA);
                    AddSize(ref SerBuf, 1);
                    SerBuf.Add((byte)(forward));
                    i2c.Write(sla, SerBuf.ToArray());

                    byte[] r = readData(MAX_FREQUENCIES * 2);

                    Buffer.BlockCopy(r, 0, m_upower, 0, MAX_FREQUENCIES);

                    for (int i = 0; i < MAX_FREQUENCIES; i++)
                    {
                        m_power[i] = (float)(m_upower[i] / 100.0);
                    }

                    return m_power;
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }

        public override void readADC(out ushort forward, out ushort reflected)
        {

            lock(m_lock)
            {
                try
                {
                     
                    forward = 0; reflected = 0;
                    List<byte> SerBuf = new List<byte>();

                    AddPrehumble(ref SerBuf);                   
                    AddOpcode(ref SerBuf, OPCODES.OPC_READ_DETECTOR);
                    AddSize(ref SerBuf, 0);

                    i2c.Write(sla, SerBuf.ToArray());
                    Thread.Sleep(10); // because of the response we need to wait 
                    ADCResponse r = readADCResponse();
                    forward = r.forward;
                    reflected = r.reflected;
                    if (r.opcode != (ushort)OPCODES.OPC_READ_DETECTOR)
                    {
                        throw (new SystemException("Data is not valid"));
                    }
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
                    List<byte> SerBuf = new List<byte>();

                    AddPrehumble(ref SerBuf);
                    AddOpcode(ref SerBuf, OPCODES.OPC_SET_MAGNITUDE);
                    AddSize(ref SerBuf, 2);

                    ushort x = mag;
                    SerBuf.Add((byte)((x >> 8) & 0xFF));
                    SerBuf.Add((byte)(x & 0xFF));

                    i2c.Write(sla, SerBuf.ToArray());

                    Thread.Sleep(2); // because of the response we need to wait 
                    readCommandResponse(OPCODES.OPC_SET_MAGNITUDE);

                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }

        public override void Close()
        {
            try
            {
                // Disconnect from DLN server
                Library.DisconnectAll();
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
    }
}
