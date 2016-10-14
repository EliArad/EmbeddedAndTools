using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using WcfTopazServiceLibrary;

namespace TopazWCFHostServiceConsoleApp
{
    class Program
    {
        static ServiceHost host = null;
        static void Main(string[] args)
        {
            try
            {
                OpenService();
                Console.WriteLine("Http service started at: http://localhost:8000/");
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                Console.ReadKey();
                return;
            }

            Console.ReadLine();
            Close();

        }
        static void OpenService()
        {

            try
            {

                WebServiceHost host = new WebServiceHost(typeof(Service1), new Uri("http://localhost:8000/"));

                ServiceEndpoint ep = host.AddServiceEndpoint(typeof(IService1), new WebHttpBinding(), "");


                // Enable metadata publishing.
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);

                // Open the ServiceHost to start listening for messages. Since
                // no endpoints are explicitly configured, the runtime will create
                // one endpoint per base address for each service contract implemented
                // by the service.
                host.Open();
            }
            catch (Exception err)
            {
                throw (new SystemException(err.Message));
            }
        }
        public static void Close()
        {
            // Close the ServiceHost.
            if (host != null)
                host.Close();
        }     
    }
}
