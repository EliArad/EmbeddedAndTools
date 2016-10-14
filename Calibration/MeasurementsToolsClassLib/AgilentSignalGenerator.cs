using System;
using System.Collections.Generic;
using System.Text;
using Ivi.Driver.Interop;
using Agilent.AgilentRfSigGen.Interop;
using Ivi.RFSigGen.Interop;

// example 
//drvr.RF.Frequency = 1E9;      // set frequency to 1GHz
//drvr.RF.Frequency = 800000000;      // set frequency to 1GHz
//drvr.RF.OutputEnabled = true; // output on

namespace AgilentSignalGeneratorLib
{
    
    public class AgilentSignalGenerator
    {
        AgilentRfSigGen driver = null;  // Version independent
        IIviRFSigGen drvr = null; 
        string vname;

        public AgilentSignalGenerator(string visa_name)
        {
            vname = visa_name;
            init(vname); 

        }
        public AgilentSignalGenerator()
        {
            vname = "USB0::0x0957::0x1F01::MY49060578::0::INSTR";
            init(vname); 

        }
    
        public  double Frequency
        {
            set {
                drvr.RF.Frequency = value;
            }
        }
        public double Level
        {
            set {
                drvr.RF.Level = value;
            }
        }
        public bool OutputEnabled
        {
            set{
                drvr.RF.OutputEnabled = value;
            }
        }



        private void init(string visa_name)
        {
            				// Create driver instance
				driver = new AgilentRfSigGen();
				// Class compliant interface (implemented by Agilent's interface)
				drvr = (IIviRFSigGen)driver;	

				// IIviDriverIdentity properties - Initialize not required
				string identifier = driver.Identity.Identifier;
				Console.WriteLine("Identifier: {0}", identifier);
				
				string revision = driver.Identity.Revision;
				Console.WriteLine("Revision: {0}", revision);
				
				string vendor = driver.Identity.Vendor;
				Console.WriteLine("Vendor: {0}", vendor);

				// Setup VISA resource descriptor.  Ignored if Simulate=true
                string resourceDesc = visa_name;
				
				// Setup IVI-defined initialization options
				string standardInitOptions = 
					"QueryInstrStatus=true, Simulate=false";
				
				// Setup driver-specific initialization options
				string driverSetupOptions = 
					"DriverSetup= Model=, Trace=false";

				driver.Initialize(resourceDesc, false, true, standardInitOptions + "," + driverSetupOptions);
				Console.WriteLine("  Driver Initialized");

				// IIviDriverIdentity properties - Initialize required
				string instModel = driver.Identity.InstrumentModel;
				Console.WriteLine("InstrumentModel: {0}", instModel);
				
				string instFirmwareRevision = driver.Identity.InstrumentFirmwareRevision;
				Console.WriteLine("InstrumentFirmwareRevision: {0}", instFirmwareRevision);
				
				string instManufacturer = driver.Identity.InstrumentManufacturer;
				Console.WriteLine("InstrumentManufacturer: {0}\n", instManufacturer);
				

				Console.WriteLine("Presetting the source.");
				drvr.Utility.Reset();

				Console.WriteLine("Setting output signal to 1GHz/0dBm" );
				drvr.Utility.Reset();

        }

    }
}
