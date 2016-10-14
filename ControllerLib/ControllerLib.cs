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
using USB_ISSApi;
 

namespace TopazControllerLib
{
    public class USBIssController : ITopazControllerApi
    { 
        
        enum IssCmds
        {
            ISS_VER = 1, 			// returns version num, 1 byte
            ISS_MODE,				// returns ACK, NACK, 1 byte
            GET_SER_NUM,

            I2C_SGL = 0x53,		    // 0x53 Read/Write single byte for non-registered devices
            I2C_AD0,				// 0x54 Read/Write multiple bytes for devices without internal address register
            I2C_AD1,				// 0x55 Read/Write multiple bytes for 1 byte addressed devices 
            I2C_AD2,				// 0x56 Read/Write multiple bytes for 2 byte addressed devices
            I2C_DIRECT,				// 0x57 Direct control of I2C start, stop, read, write.
            ISS_CMD = 0x5A,		    // 0x5A 
            SPI_IO = 0x61,			// 0x61 SPI I/O
            SERIAL_IO,              // 0x62
            SETPINS,				// 0x63 [SETPINS] [pin states]
            GETPINS,				// 0x64 
            GETAD,					// 0x65 [GETAD] [pin to convert]
        };

        // I2C DIRECT commands
        enum I2Cdirect
        {
            I2CSRP = 0x00,			// Start/Stop Codes - 0x01=start, 0x02=restart, 0x03=stop, 0x04=nack
            I2CSTART,				// send start sequence
            I2CRESTART,				// send restart sequence
            I2CSTOP,				// send stop sequence
            I2CNACK,				// send NACK after next read
            I2CREAD = 0x20,		    // 0x20-0x2f, reads 1-16 bytes
            I2CWRITE = 0x30,		// 0x30-0x3f, writes next 1-16 bytes
        };

        const byte m_write_i2c_address = 0x7F;
        const byte m_write_i2c_address_shift = (0x7F << 1);
        const byte m_read_i2c_address_shift = (byte)((0x7F << 1) + 1);
        
        ISSComm comm;

        bool m_saveAsText = false;
        public USBIssController()
        {
            comm = ISSComm.getComm();
        }
        
       
        public override void connect(string comPort, int BaudRate, bool autoDetect = false)
        {
            try
            {

                if (autoDetect == true)
                {
                    comPort = GetUSB_ISS_PortName();
                }

                if (ISSComm.getComm().connect(comPort, BaudRate) == false)
                    throw (new SystemException("Failed to open comport " + comPort));

              
                ISSComm.ISS_VERSION data = new ISSComm.ISS_VERSION();

                if ((!data.isValid) || (data.moduleID != 7))
                { 
                    // if the module id is not that of the USB-ISS
                    throw (new SystemException("Device not found"));
                }

                string txtMode;

                string lblDeviceData = "USB-ISS V" + data.fwVersion + ", SN: " + (new ISSComm.GET_SER_NUM()).getSerNum(); //print the software version on screen
                

                switch (data.operMode & 0xFE)
                {
                    case (int)ISSComm.ISS_MODE.ISS_MODES.IO_MODE: txtMode = "IO_MODE"; break;
                    case (int)ISSComm.ISS_MODE.ISS_MODES_I2C.I2C_H_1000KHZ: txtMode = "I2C 1MHz HW"; break;
                    case (int)ISSComm.ISS_MODE.ISS_MODES_I2C.I2C_H_100KHZ: txtMode = "I2C 100KHz HW"; break;
                    case (int)ISSComm.ISS_MODE.ISS_MODES_I2C.I2C_H_400KHZ: txtMode = "I2C 400KHz HW"; break;
                    case (int)ISSComm.ISS_MODE.ISS_MODES_I2C.I2C_S_100KHZ: txtMode = "I2C 100KHz SW"; break;
                    case (int)ISSComm.ISS_MODE.ISS_MODES_I2C.I2C_S_20KHZ: txtMode = "I2C 20KHz SW"; break;
                    case (int)ISSComm.ISS_MODE.ISS_MODES_I2C.I2C_S_500KHZ: txtMode = "I2C 500KHz SW"; break;
                    case (int)ISSComm.ISS_MODE.ISS_MODES_I2C.I2C_S_50KHZ: txtMode = "I2C 50KHz SW"; break;
                    case (int)ISSComm.ISS_MODE.ISS_MODES_SPI.A2I_L: txtMode = "SPI TX on Act->Idle, Clock idle = low"; break;
                    case (int)ISSComm.ISS_MODE.ISS_MODES_SPI.A2I_H: txtMode = "SPI TX on Act->Idle, Clock idle = high"; break;
                    case (int)ISSComm.ISS_MODE.ISS_MODES_SPI.I2A_L: txtMode = "SPI TX on Idle->Act, Clock idle = low"; break;
                    case (int)ISSComm.ISS_MODE.ISS_MODES_SPI.I2A_H: txtMode = "SPI TX on Idle->Act, Clock idle = high"; break;
                    default: txtMode = "Unknown mode: 0x" + data.operMode.ToString("X2"); break;
                }
                if ((data.operMode & (int)ISSComm.ISS_MODE.ISS_MODES.SERIAL) == (int)ISSComm.ISS_MODE.ISS_MODES.SERIAL) txtMode += ", with Serial";

                byte speed = (byte)ISSComm.ISS_MODE.ISS_MODES_I2C.I2C_H_1000KHZ;
                //speed = data.operMod;
                

                ISSComm comm = ISSComm.getComm();
                comm.Write(new byte[] { 0x5A, 0x02, speed, 0xaa });
                comm.Read(2);

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
                    byte i = 0;
                    byte[] SerBuf = new byte[50];
                    SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
                    SerBuf[i++] = (byte)(I2Cdirect.I2CWRITE + 6 + data.Length);
                    SerBuf[i++] = m_write_i2c_address_shift;
                    SerBuf[i++] = (byte)0x52;
                    SerBuf[i++] = (byte)0x91;
                    SerBuf[i++] = (byte)((ushort)OPCODES.OPC_WRITE_FLASH_CHUNK & 0xFF);
                    SerBuf[i++] = (byte)(((ushort)OPCODES.OPC_WRITE_FLASH_CHUNK >> 8) & 0xFF);

                    ushort size = (ushort)(data.Length);
                    SerBuf[i++] = (byte)(size & 0xFF);
                    SerBuf[i++] = (byte)((size >> 8) & 0xFF);

                    for (int j = 0; j < size; j++)
                    {
                        SerBuf[i++] = data[j];
                    }

                    SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;
                    comm.Write(SerBuf, i);
                    int toRead = CheckResponse();
                    Thread.Sleep(1); // because of the response we need to wait 
                    readCommandResponse(OPCODES.OPC_WRITE_FLASH_CHUNK);
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }
        public override DR[] GetDR()
        {
            throw new NotImplementedException();
        }
        public override void SendDataToController(List<ushort> list)
        {
            ushort[] toSend;
            list.Add(0xF1F2); // we mark the end of the list for the controller to east detect it
            list.Insert(0, (ushort)list.Count);
            ushort[] data = list.ToArray();
            int sizeToSend = list.Count;
            int index = 0;
            bool startAlgoDownload = true;
            int transferCount = 4;

            while (sizeToSend > 0)
            {
                int min = Math.Min(sizeToSend, transferCount);
                toSend = new ushort[min];
                Array.Copy(data, index, toSend, 0, min);
                AddAlgo(toSend, startAlgoDownload);
                sizeToSend -= min;
                index += min;
                startAlgoDownload = false;
            }
        }
        public override void Start(ushort totalSeconds, bool oneTime)
        {
            lock (m_lock)
            {
                try
                {
                    byte i = 0;
                    byte[] SerBuf = new byte[50];
                    SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
                    SerBuf[i++] = (byte)I2Cdirect.I2CWRITE + 6 + 0;
                    SerBuf[i++] = m_write_i2c_address_shift;
                    SerBuf[i++] = (byte)0x52;
                    SerBuf[i++] = (byte)0x91;
                    SerBuf[i++] = (byte)((ushort)OPCODES.OPC_START_COOKING & 0xFF);
                    SerBuf[i++] = (byte)(((ushort)OPCODES.OPC_START_COOKING >> 8) & 0xFF);
                    
                    ushort size = 0;
                    SerBuf[i++] = (byte)(size & 0xFF);
                    SerBuf[i++] = (byte)((size >> 8) & 0xFF);
                     
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;
                    comm.Write(SerBuf, i);
                    int toRead = CheckResponse();
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

                    byte i = 0;
                    byte[] SerBuf = new byte[50];
                    SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
                    SerBuf[i++] = (byte)I2Cdirect.I2CWRITE + 6;
                    SerBuf[i++] = m_write_i2c_address_shift;
                    SerBuf[i++] = (byte)0x52;
                    SerBuf[i++] = (byte)0x91;
                    SerBuf[i++] = (byte)((ushort)OPCODES.OPC_STOP_COOKING & 0xFF);
                    SerBuf[i++] = (byte)(((ushort)OPCODES.OPC_STOP_COOKING >> 8) & 0xFF);
                    ushort size = 0;
                    SerBuf[i++] = (byte)(size & 0xFF);
                    SerBuf[i++] = (byte)((size >> 8) & 0xFF);

                    SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;
                    comm.Write(SerBuf, i);
                    int toRead = CheckResponse();
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

                    byte i = 0;
                    byte[] SerBuf = new byte[50];
                    SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
                    SerBuf[i++] = (byte)I2Cdirect.I2CWRITE + 6 + 2;
                    SerBuf[i++] = m_write_i2c_address_shift;
                    SerBuf[i++] = (byte)0x52;
                    SerBuf[i++] = (byte)0x91;
                    SerBuf[i++] = (byte)((ushort)OPCODES.OPC_SET_FREQUENCY & 0xFF);
                    SerBuf[i++] = (byte)(((ushort)OPCODES.OPC_SET_FREQUENCY >> 8) & 0xFF);
                    ushort size = 2;
                    SerBuf[i++] = (byte)(size & 0xFF);
                    SerBuf[i++] = (byte)((size >> 8) & 0xFF);

                    SerBuf[i++] = (byte)(dishId & 0xFF);
                    SerBuf[i++] = (byte)((dishId >> 8) & 0xFF);


                    SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;
                    comm.Write(SerBuf, i);
                    int toRead = CheckResponse();
                    Thread.Sleep(1); // because of the response we need to wait 
                    readCommandResponse(OPCODES.OPC_SET_FREQUENCY);

                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }


        public override void Pause(bool pause)
        {
            lock (m_lock)
            {
                try
                {

                    byte i = 0;
                    byte[] SerBuf = new byte[50];
                    SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
                    SerBuf[i++] = (byte)I2Cdirect.I2CWRITE + 6 + 2;
                    SerBuf[i++] = m_write_i2c_address_shift;
                    SerBuf[i++] = (byte)0x52;
                    SerBuf[i++] = (byte)0x91;
                    SerBuf[i++] = (byte)((ushort)OPCODES.OPC_PAUSE_COOKING & 0xFF);
                    SerBuf[i++] = (byte)(((ushort)OPCODES.OPC_PAUSE_COOKING >> 8) & 0xFF);
                    ushort size = 2;
                    SerBuf[i++] = (byte)(size & 0xFF);
                    SerBuf[i++] = (byte)((size >> 8) & 0xFF);
                    ushort x = (ushort)(pause == true ? 1 : 0);
                    SerBuf[i++] = (byte)(x & 0xFF);
                    SerBuf[i++] = (byte)((x >> 8) & 0xFF);
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;
                    comm.Write(SerBuf, i);
                    int toRead = CheckResponse();
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

                    byte i = 0;
                    byte[] SerBuf = new byte[50];
                    SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
                    SerBuf[i++] = (byte)I2Cdirect.I2CWRITE + 6 + 2;
                    SerBuf[i++] = m_write_i2c_address_shift;
                    SerBuf[i++] = (byte)0x52;
                    SerBuf[i++] = (byte)0x91;
                    SerBuf[i++] = (byte)((ushort)OPCODES.OPC_SET_FREQUENCY & 0xFF);
                    SerBuf[i++] = (byte)(((ushort)OPCODES.OPC_SET_FREQUENCY >> 8) & 0xFF);
                    ushort size = 2;
                    SerBuf[i++] = (byte)(size & 0xFF);
                    SerBuf[i++] = (byte)((size >> 8) & 0xFF);
                    ushort x = (ushort)((frequency - 2400) * 2);
                    SerBuf[i++] = (byte)(x & 0xFF);
                    SerBuf[i++] = (byte)((x >> 8) & 0xFF);
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;
                    comm.Write(SerBuf, i);
                    int toRead = CheckResponse();
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
                    byte i = 0;
                    byte[] SerBuf = new byte[50];

                    SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
                    SerBuf[i++] = (byte)(I2Cdirect.I2CWRITE + 6);
                    SerBuf[i++] = m_write_i2c_address_shift;
                    SerBuf[i++] = (byte)0x52;
                    SerBuf[i++] = (byte)0x91;
                    SerBuf[i++] = (byte)((ushort)OPCODES.OPC_READ_TOPAZ_INFO & 0xFF);
                    SerBuf[i++] = (byte)(((ushort)OPCODES.OPC_READ_TOPAZ_INFO >> 8) & 0xFF);
                    SerBuf[i++] = (byte)(0 & 0xFF);
                    SerBuf[i++] = (byte)((0 >> 8) & 0xFF);
                     
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;
                    comm.Write(SerBuf, i);
                    int toRead = CheckResponse();
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
                    byte i = 0;
                    byte[] SerBuf = new byte[50];
                
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

                    int size = b1Data.Length;// +b2.Length;

                    SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
                    SerBuf[i++] = (byte)(I2Cdirect.I2CWRITE + 6 + size);
                    SerBuf[i++] = m_write_i2c_address_shift;
                    SerBuf[i++] = (byte)0x52;
                    SerBuf[i++] = (byte)0x91;
                     
                    SerBuf[i++] = (byte)((ushort)OPCODES.OPC_WRITE_TOPAZ_INFO & 0xFF);
                    SerBuf[i++] = (byte)(((ushort)OPCODES.OPC_WRITE_TOPAZ_INFO >> 8) & 0xFF);
                     
                    SerBuf[i++] = (byte)(size & 0xFF);
                    SerBuf[i++] = (byte)((size >> 8) & 0xFF);
                     
                    // Data start here:
                    for (int j = 0; j < b1Data.Length; j++)
                        SerBuf[i++] = b1Data[j];
                    /*
                    for (int j = 0; j < b2.Length; j++)
                        SerBuf[i++] = b1[j];
                    */
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;
                    comm.Write(SerBuf, i);
                    int toRead = CheckResponse();
                    Thread.Sleep(2); // because of the response we need to wait 
                    readCommandResponse(OPCODES.OPC_WRITE_TOPAZ_INFO);
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }
        public override void AddAlgo(ushort[] list, bool start)
        {
            lock (m_lock)
            {
                try
                {
                    byte i = 0;
                    byte[] SerBuf = new byte[50 + list.Length];

                    SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
                    SerBuf[i++] = (byte)(I2Cdirect.I2CWRITE + 6 + (byte)(list.Count() * 2));
                    SerBuf[i++] = m_write_i2c_address_shift;
                    SerBuf[i++] = (byte)0x52;
                    SerBuf[i++] = (byte)0x91;
                    OPCODES opcode;
                    if (start == true)
                    {
                        opcode = OPCODES.OPC_ADDALGO; 
                    }
                    else
                    {
                        opcode = OPCODES.OPC_APPENDALGO; 
                    }

                    SerBuf[i++] = (byte)((ushort)opcode & 0xFF);
                    SerBuf[i++] = (byte)(((ushort)opcode >> 8) & 0xFF);


                    ushort size = (ushort)(list.Count() * 2);
                    SerBuf[i++] = (byte)(size & 0xFF);
                    SerBuf[i++] = (byte)((size >> 8) & 0xFF);

                    for (int j = 0; j < list.Length; j++)
                    {
                        SerBuf[i++] = (byte)(list[j] & 0xFF);
                        SerBuf[i++] = (byte)((list[j] >> 8) & 0xFF);
                    }

                    SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;

                    comm.Write(SerBuf, i);
                    int toRead = CheckResponse();
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
            byte i = 0;
            byte[] SerBuf = new byte[50];
            
            SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
            SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
            SerBuf[i++] = (byte)I2Cdirect.I2CWRITE;
            SerBuf[i++] = m_write_i2c_address_shift;

            SerBuf[i++] = (byte)I2Cdirect.I2CRESTART;
            SerBuf[i++] = (byte)I2Cdirect.I2CWRITE;
            SerBuf[i++] = m_read_i2c_address_shift;
            SerBuf[i++] = (byte)I2Cdirect.I2CREAD + 6;
            SerBuf[i++] = (byte)I2Cdirect.I2CNACK;
            SerBuf[i++] = (byte)I2Cdirect.I2CREAD;
            SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;
            comm.Write(SerBuf, i);
            int toRead = CheckResponse();
            comm.Read(toRead, SerBuf);
            TopazHwInfo r = new TopazHwInfo();
                        
            return r;
        }

        void readCommandResponse(OPCODES opcode)
        {
            byte i = 0;
            byte[] SerBuf = new byte[50];
            
            SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
            SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
            SerBuf[i++] = (byte)I2Cdirect.I2CWRITE;
            SerBuf[i++] = m_write_i2c_address_shift;

            SerBuf[i++] = (byte)I2Cdirect.I2CRESTART;
            SerBuf[i++] = (byte)I2Cdirect.I2CWRITE;
            SerBuf[i++] = m_read_i2c_address_shift;
            SerBuf[i++] = (byte)I2Cdirect.I2CREAD + 2;
            SerBuf[i++] = (byte)I2Cdirect.I2CNACK;
            SerBuf[i++] = (byte)I2Cdirect.I2CREAD;
            SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;
            comm.Write(SerBuf, i);
            int toRead = CheckResponse();
            comm.Read(toRead, SerBuf);
            Response r = new Response();
            ByteArrayToStruct(SerBuf, ref r);
            if (r.opcode != (ushort)opcode || r.res != 1)
            {   
                throw (new SystemException("Incorrect response"));
            }
        }
        StatusResponse readStatusResponse()
        {
            byte i = 0;
            byte[] SerBuf = new byte[50];

            SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
            SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
            SerBuf[i++] = (byte)I2Cdirect.I2CWRITE;
            SerBuf[i++] = m_write_i2c_address_shift;

            SerBuf[i++] = (byte)I2Cdirect.I2CRESTART;
            SerBuf[i++] = (byte)I2Cdirect.I2CWRITE;
            SerBuf[i++] = m_read_i2c_address_shift;
            SerBuf[i++] = (byte)I2Cdirect.I2CREAD + 14;
            SerBuf[i++] = (byte)I2Cdirect.I2CNACK;
            SerBuf[i++] = (byte)I2Cdirect.I2CREAD;
            SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;
            comm.Write(SerBuf, i);
            int toRead = CheckResponse();
            if (toRead > 0)
                comm.Read(toRead, SerBuf);
            StatusResponse r = new StatusResponse();
            ByteArrayToStruct(SerBuf, ref r);
            return r;
        }

        public ADCResponse readADCResponse()
        {
            byte i = 0;
            byte[] SerBuf = new byte[50];

            SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
            SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
            SerBuf[i++] = (byte)I2Cdirect.I2CWRITE;
            SerBuf[i++] = m_write_i2c_address_shift;

            SerBuf[i++] = (byte)I2Cdirect.I2CRESTART;
            SerBuf[i++] = (byte)I2Cdirect.I2CWRITE;
            SerBuf[i++] = m_read_i2c_address_shift;
            SerBuf[i++] = (byte)I2Cdirect.I2CREAD + 4;
            SerBuf[i++] = (byte)I2Cdirect.I2CNACK;
            SerBuf[i++] = (byte)I2Cdirect.I2CREAD;
            SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;
            comm.Write(SerBuf, i);
            int toRead = CheckResponse();
            if (toRead > 0)
                comm.Read(toRead, SerBuf);
            ADCResponse r = new ADCResponse();
            ByteArrayToStruct(SerBuf, ref r);             
            return r;
        }

        byte [] readData(int size)
        {
            byte i = 0;
            byte[] SerBuf = new byte[50];

            SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
            SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
            SerBuf[i++] = (byte)I2Cdirect.I2CWRITE;
            SerBuf[i++] = m_write_i2c_address_shift;

            SerBuf[i++] = (byte)I2Cdirect.I2CRESTART;
            SerBuf[i++] = (byte)I2Cdirect.I2CWRITE;
            SerBuf[i++] = m_read_i2c_address_shift;
            SerBuf[i++] = (byte)(I2Cdirect.I2CREAD + (size - 2));
            SerBuf[i++] = (byte)I2Cdirect.I2CNACK;
            SerBuf[i++] = (byte)I2Cdirect.I2CREAD;
            SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;
            comm.Write(SerBuf, i);
            int toRead = CheckResponse();
            if (toRead > 0)
                comm.Read(toRead, SerBuf);            
            return SerBuf;
        }

        int CheckResponse()
        {
            byte[] SerBuf = new byte[10];
            comm.Read(2, SerBuf);
            if (SerBuf[0] == 0)
            {
                switch (SerBuf[1])
                {
                    case 0x1:
                        throw (new SystemException("Device Error	0x01	No ACK from device"));

                    case 0x2:
                        throw (new SystemException("Buffer Overflow	0x02	You must limit the frame to < 60 bytes"));

                    case 0x3:
                        throw (new SystemException("Buffer Underflow	0x03	More write data was expected than sent"));

                    case 0x4:
                        throw (new SystemException("Unknown command	0x04	Probably your write count is wrong"));
                    default:
                        throw (new SystemException("unknown error"));
                }
            }
            else
            {
                return SerBuf[1];
            }
        }
        public ushort getMagnitude()
        {

            return 0;
        }

        public ushort getFrequency()
        {

            return 0;
        }

        public ushort getMse()
        {

            return 0;
        }
        public void setMse()
        {

        }
        public override StatusResponse ReadStatus()
        {
            lock (m_lock)
            {
                try
                {
               
                    byte i = 0;
                    byte[] SerBuf = new byte[50];
                    SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
                    SerBuf[i++] = (byte)I2Cdirect.I2CWRITE + 6;
                    SerBuf[i++] = m_write_i2c_address_shift;
                    SerBuf[i++] = (byte)0x52;
                    SerBuf[i++] = (byte)0x91;
                    SerBuf[i++] = (byte)((ushort)OPCODES.OPC_READ_STATUS & 0xFF);
                    SerBuf[i++] = (byte)(((ushort)OPCODES.OPC_READ_STATUS >> 8) & 0xFF);
                    SerBuf[i++] = 0;
                    SerBuf[i++] = 0;
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;
                    comm.Write(SerBuf, i);
                    int toRead = CheckResponse();
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
                    byte i = 0;
                    byte[] SerBuf = new byte[50];
                    SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
                    SerBuf[i++] = (byte)I2Cdirect.I2CWRITE + 6 + 2;
                    SerBuf[i++] = m_write_i2c_address_shift;
                    SerBuf[i++] = (byte)0x52;
                    SerBuf[i++] = (byte)0x91;
                    SerBuf[i++] = (byte)((ushort)OPCODES.OPC_READ_FLASH_CHUNK & 0xFF);
                    SerBuf[i++] = (byte)(((ushort)OPCODES.OPC_READ_FLASH_CHUNK >> 8) & 0xFF);
                    ushort size = 2;
                    SerBuf[i++] = (byte)(size & 0xFF);
                    SerBuf[i++] = (byte)((size >> 8) & 0xFF);

                    SerBuf[i++] = (byte)((int)location & 0xFF);
                    SerBuf[i++] = (byte)(((int)location >> 8) & 0xFF);

                    SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;
                    comm.Write(SerBuf, i);
                    int toRead = CheckResponse();
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
                    for (int n = 0; n < 2; n++)
                    {
                        FLASH_LOCATION fl = n == 0 ? FLASH_LOCATION.AB_FORWARD : FLASH_LOCATION.AB_REFLECTED;
                        FlashHeader header = ReadFlashHeaderSize(fl);
                        int CHUNK_SIZE = 8;
                        StreamWriter sw = new StreamWriter(fileName[n]);
                        for (int i = 0; i < 101; i++)
                        {                             
                            byte[] data = ReadChunk(CHUNK_SIZE);
                            double a = BitConverter.ToDouble(data, 0);
                            sw.Write("{0},", a);
                            data = ReadChunk(CHUNK_SIZE);
                            double b = BitConverter.ToDouble(data, 0);
                            sw.WriteLine("{0}", b);
                        }
                        sw.Close();
                    }
                }
                catch (Exception err)
                {
                    throw (new SystemException(err.Message));
                }
            }
        }
        byte [] ReadChunk(int chunkSize)
        {

            lock (m_lock)
            {
                try
                {
                    byte i = 0;
                    byte[] SerBuf = new byte[50];
                    SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
                    SerBuf[i++] = (byte)I2Cdirect.I2CWRITE + 6 + 2;
                    SerBuf[i++] = m_write_i2c_address_shift;
                    SerBuf[i++] = (byte)0x52;
                    SerBuf[i++] = (byte)0x91;
                    SerBuf[i++] = (byte)((ushort)OPCODES.OPC_READ_FLASH_CHUNK & 0xFF);
                    SerBuf[i++] = (byte)(((ushort)OPCODES.OPC_READ_FLASH_CHUNK >> 8) & 0xFF);
                    ushort size = 2;
                    SerBuf[i++] = (byte)(size & 0xFF);
                    SerBuf[i++] = (byte)((size >> 8) & 0xFF);
                     
                    SerBuf[i++] = (byte)(chunkSize & 0xFF);
                    SerBuf[i++] = (byte)((chunkSize >> 8) & 0xFF);


                    SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;
                    comm.Write(SerBuf, i);
                    int toRead = CheckResponse();
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

        public override void readADC(out ushort forward, out ushort reflected)
        {

            lock (m_lock)
            {
                try
                {
                    forward = 0; reflected = 0;
                    byte i = 0;
                    byte[] SerBuf = new byte[50];
                    SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
                    SerBuf[i++] = (byte)I2Cdirect.I2CWRITE + 6;
                    SerBuf[i++] = m_write_i2c_address_shift;
                    SerBuf[i++] = (byte)0x52;
                    SerBuf[i++] = (byte)0x91;
                    SerBuf[i++] = (byte)((ushort)OPCODES.OPC_READ_DETECTOR & 0xFF);
                    SerBuf[i++] = (byte)(((ushort)OPCODES.OPC_READ_DETECTOR >> 8) & 0xFF);
                    SerBuf[i++] = 0;
                    SerBuf[i++] = 0;
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;
                    comm.Write(SerBuf, i);
                    int toRead = CheckResponse();
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
                    byte i = 0;
                    byte[] SerBuf = new byte[50];
                    SerBuf[i++] = (byte)IssCmds.I2C_DIRECT;
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTART;
                    SerBuf[i++] = (byte)I2Cdirect.I2CWRITE + 8;
                    SerBuf[i++] = m_write_i2c_address_shift;
                    SerBuf[i++] = (byte)0x52;
                    SerBuf[i++] = (byte)0x91;
                    SerBuf[i++] = (byte)((ushort)OPCODES.OPC_SET_MAGNITUDE & 0xFF);
                    SerBuf[i++] = (byte)(((ushort)OPCODES.OPC_SET_MAGNITUDE >> 8) & 0xFF);

                    ushort size = 2;
                    SerBuf[i++] = (byte)(size & 0xFF);
                    SerBuf[i++] = (byte)((size >> 8) & 0xFF);
                    ushort x = mag;
                    SerBuf[i++] = (byte)((x >> 8) & 0xFF);
                    SerBuf[i++] = (byte)(x & 0xFF);
                    SerBuf[i++] = (byte)I2Cdirect.I2CSTOP;

                    comm.Write(SerBuf, i);
                    int toRead = CheckResponse();
                    Thread.Sleep(2); // because of the response we need to wait 
                    readCommandResponse(OPCODES.OPC_SET_MAGNITUDE);

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


        public override void Close()
        {
            try
            {
                ISSComm.getComm().Close();
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
    }
}
