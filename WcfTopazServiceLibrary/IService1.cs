using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfTopazServiceLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]    
    public interface IService1
    {
        [OperationContract]        
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "Mult?x={x}&y={y}", 
            BodyStyle = WebMessageBodyStyle.Bare)]
        long Multiply(long x, long y);

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "Mult2?x={x}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        Stream Multiply2(string x);

         
        [OperationContract]
        [WebGet]
        Stream setFrequency(ushort freq);


        [OperationContract]
        [WebGet]
        Stream connect();

        [OperationContract]
        [WebGet]
        Stream Close();



        [OperationContract]
        [WebGet]
        Stream setMagnitude(ushort mag);
        


        [OperationContract]
        [WebGet]
        Stream SendDataToController(string data);

        [OperationContract]
        [WebGet]
        Stream ReadStatus();

        [OperationContract]
        [WebGet]
        Stream ReadCalibration();

        [OperationContract]
        [WebGet]         
        Stream readADC();

        //DR[] GetDR();

        //float[] GetAgcPower(byte forward);

        [OperationContract]
        [WebGet]
        Stream Stop();

        [OperationContract]
        [WebGet]
        Stream Start(ushort totalSeconds, bool oneTime);

        [OperationContract]
        [WebGet]
        Stream StartDish(ushort dishId);

        [OperationContract]
        [WebGet]
        Stream Pause(bool pause);



        [OperationContract]
        [WebGet]
        Stream Reset();


        [OperationContract]
        [WebGet]
        Stream SendBreak(string ComPort);



        // TODO: Add your service operations here
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "WcfTopazServiceLibrary.ContractType".
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
