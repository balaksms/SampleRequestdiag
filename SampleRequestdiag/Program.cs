using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Linq;
namespace SampleRequestdiag
{
    public class customer
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string CustomerName { get; set; }
        public string Region { get; set; }
        
    }
    

    class Program
    {
        public const string RequestDiagString = "Request Diagnostic String : {0}";
        public const string RequestLatencyStats = "Request Latency Stats : {0}";
        public const string RequestChargeStats = "Request Charge Stats: {0}";
        public const string RequestQueryMetrics = "Request Query Metric Stats: {0}";

        public static int m_maxdoccount = 5;
        public static string m_szLogFileName = "log\\DebugLog.txt";
        private static string m_szDatabaseName = "dbrequestdiagdemo";
        private static string m_szCollectionName = "crequestdiagdemo";
        private static string m_szAccountURL = "https://testmigrationtool.documents.azure.com:443/";
        private static string m_szAccountKey = "input database account key";
        //adding comment
        private static DocumentClient m_DC = null;
        static void Main(string[] args)
        {
            try
            {
               // List<customer> ret = new List<customer>();
                m_szLogFileName = ConfigurationManager.AppSettings.Get("LogFile");

                if (string.IsNullOrEmpty(m_szLogFileName) || string.IsNullOrWhiteSpace(m_szLogFileName))
                {
                    // setting default Debug File Name
                    m_szLogFileName = "log\\DebugLog.txt";
                }
                m_szLogFileName = string.Format("{0}_{1:yyyy_MM_dd_hh_mm_ss_tt}.txt", m_szLogFileName, DateTime.Now);
                if(InitializeConnection())
                {
                    if(CreateDBCollection().Result)
                    { 
                        CreateDoc().Wait();
                        List<customer> ret = QueryCustomer().Result;
                        ReadDoc(ret).Wait();
                        ReplaceDoc(ret).Wait();
                        UpsertDoc(ret).Wait();
                        DeleteDoc(ret).Wait();
                    }
                    else
                    {
                        writelog("Create db/collection failed");
                    }
                }
                else
                {
                    writelog("Connection failed");
                }

            }
            catch (Exception ex)
            {
                writelog(ex.Message.ToString());

                if (ex.InnerException != null)
                {
                    writelog(ex.InnerException.Message.ToString());
                }

            }
        }
        private static bool InitializeConnection()
        {
         
            Console.SetWindowSize(100, 45);
            m_szDatabaseName = ConfigurationManager.AppSettings.Get("DatabaseName");
            m_szCollectionName = ConfigurationManager.AppSettings.Get("CollectionName");
            m_szAccountURL = ConfigurationManager.AppSettings.Get("AccountURL");
            m_szAccountKey = ConfigurationManager.AppSettings.Get("AccountKey");

            try
            {
                    m_DC = new DocumentClient(new Uri(m_szAccountURL), m_szAccountKey, new ConnectionPolicy()
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp,
                    //RetryOptions = new RetryOptions() { MaxRetryAttemptsOnThrottledRequests = 0 },
                });
                m_DC.OpenAsync().Wait();
                return true;
            }
            catch (DocumentClientException dex )
            {
                writelog(dex.Message.ToString());
                return false;
            }
            catch (Exception ex)
            {
                writelog(ex.Message.ToString());
                return false;
            }
            
        }

        private static async Task<bool> CreateDBCollection()
        {
            DocumentCollection m_coll = new DocumentCollection();
            DocumentCollection myCollection = new DocumentCollection();
            myCollection.Id = m_szCollectionName;
            myCollection.PartitionKey.Paths.Add("/Region");


            Database database = await m_DC.CreateDatabaseIfNotExistsAsync(new Database { Id = m_szDatabaseName });

            try
            {
                m_coll = await m_DC.CreateDocumentCollectionIfNotExistsAsync(
                   database.SelfLink,
                   myCollection,
                   new RequestOptions { OfferThroughput = 400 });
            }
            catch (DocumentClientException ex)
            {
                writelog(ex.Message.ToString());
                return false;
            }
            return true;
        }
        

        private static customer GetCustomerDoc(int id)
        {
            customer citem = new customer();
            citem.Id = id.ToString();
            citem.CustomerName = "Microsoft";
            citem.Region = "West US";
            return citem;
        }


        private static async Task CreateDoc()
        {
            
            try
            {
                for(int icount=1; icount<= m_maxdoccount; icount++)
                {
                    customer citem = GetCustomerDoc(icount);
                    ResourceResponse<Document> cxResponse = await m_DC.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(m_szDatabaseName, m_szCollectionName), citem, new RequestOptions { PartitionKey = new PartitionKey(citem.Region) });
                    writelog(string.Format(RequestDiagString, cxResponse.RequestDiagnosticsString.ToString()));
                    writelog(string.Format(RequestLatencyStats, cxResponse.RequestLatency.ToString()));
                    writelog(string.Format(RequestChargeStats, cxResponse.RequestCharge.ToString()));
                }

            }
            catch (DocumentClientException dex) 
            {
                writelog(dex.Message.ToString());
            }
            catch(Exception ex)
            {
                writelog(ex.Message.ToString());
            }

        }
        private static async Task ReplaceDoc(List<customer> pitem)
        {
            try
            {
                foreach (customer citem in pitem)
                {
                    citem.CustomerName += DateTime.Now.ToString();
                    ResourceResponse<Document> cxResponse = await m_DC.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(m_szDatabaseName, m_szCollectionName, citem.Id), citem, new RequestOptions { PartitionKey = new PartitionKey(citem.Region) });
                    writelog(string.Format(RequestDiagString, cxResponse.RequestDiagnosticsString.ToString()));
                    writelog(string.Format(RequestLatencyStats, cxResponse.RequestLatency.ToString()));
                    writelog(string.Format(RequestChargeStats, cxResponse.RequestCharge.ToString()));
                }
            }
            catch (DocumentClientException dex)
            {

                writelog(dex.Message.ToString());
            }
            catch(Exception ex)
            {
                writelog(ex.Message.ToString());
            }
        }
        private static async Task ReadDoc(List<customer> pitem)
        {
            try
            {
                foreach(customer citem in pitem)
                { 
                ResourceResponse<Document> cxResponse = await m_DC.ReadDocumentAsync(UriFactory.CreateDocumentUri(m_szDatabaseName, m_szCollectionName, citem.Id).ToString(), new RequestOptions { PartitionKey = new PartitionKey(citem.Region) });
                writelog(string.Format(RequestDiagString, cxResponse.RequestDiagnosticsString.ToString()));
                writelog(string.Format(RequestLatencyStats, cxResponse.RequestLatency.ToString()));
                writelog(string.Format(RequestChargeStats, cxResponse.RequestCharge.ToString()));
                }
            }
            catch (DocumentClientException dex)
            {
                writelog(dex.Message.ToString());


            }
            catch (Exception ex)
            {
                writelog(ex.Message.ToString());

            }
        }

        private static async Task<List<customer>> QueryCustomer()
        {
            List<customer> output = new List<customer>();
            try
            { 
                var cx = m_DC.CreateDocumentQuery<customer>(
                    UriFactory.CreateDocumentCollectionUri(m_szDatabaseName, m_szCollectionName),
                    "select * from c", new FeedOptions() { EnableCrossPartitionQuery = true, PopulateQueryMetrics = true }).AsDocumentQuery();
                
                while(cx.HasMoreResults)
                {
                    FeedResponse<customer> result = await cx.ExecuteNextAsync<customer>();
                   // writelog(string.Format(RequestDiagString, result.RequestDiagnosticsString));
                    writelog(string.Format(RequestQueryMetrics, result.QueryMetrics.ToString()));
                    writelog(string.Format(RequestChargeStats, result.RequestCharge.ToString()));
                    output.AddRange(result.ToList());
                }
            }
            catch(DocumentClientException dex)
            {
                writelog(dex.Message.ToString());
            }
            catch(Exception ex)
            {
                writelog(ex.Message.ToString());
            }
            return output;
        }

        private static async Task UpsertDoc(List<customer> pitem)
        {

            foreach(customer citem in pitem)
            {
                try
                { 
                    citem.CustomerName += citem.CustomerName + DateTime.Now.ToString();
                    ResourceResponse<Document> response = await m_DC.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(m_szDatabaseName, m_szCollectionName).ToString(), citem, new RequestOptions { PartitionKey = new PartitionKey(citem.Region) });
                    var upserted = response.Resource;
                    writelog(string.Format(RequestDiagString, response.RequestDiagnosticsString.ToString()));
                    writelog(string.Format(RequestLatencyStats, response.RequestLatency.ToString()));
                    writelog(string.Format(RequestChargeStats, response.RequestCharge.ToString()));
                }
                catch(DocumentClientException dex)
                {
                    writelog(dex.Message.ToString());
                }
                catch(Exception ex)
                {
                    writelog(ex.Message.ToString());
                }
            }
        }
        private static async Task DeleteDoc(List<customer> pitem)
        {
            foreach (customer citem in pitem)
            {
                try
                { 
                ResourceResponse<Document> response = await m_DC.DeleteDocumentAsync(
                UriFactory.CreateDocumentUri(m_szDatabaseName, m_szCollectionName, citem.Id),
                new RequestOptions { PartitionKey = new PartitionKey(citem.Region) });
                writelog(string.Format(RequestDiagString, response.RequestDiagnosticsString.ToString()));
                writelog(string.Format(RequestLatencyStats, response.RequestLatency.ToString()));
                writelog(string.Format(RequestChargeStats, response.RequestCharge.ToString()));
                }
                catch(DocumentClientException dex)
                {
                    writelog(dex.Message.ToString());
                }
                catch(Exception ex)
                {
                    writelog(ex.Message.ToString());
                }
            }

        }

        private static void writelog(string sMsg)
        {
            //Logformat = "{0:d,-10} {1,-12} | {2 ,-50} | {3,-10} | {4,-50}";
            string Logformat = "{0:d,-10} {1,-12} | {2,-50}"; ;
            string appending = String.Format(Logformat, DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.ToLongTimeString(),sMsg);
            appending = appending + string.Format("{0}", System.Environment.NewLine);
            string foldername = System.IO.Path.GetDirectoryName(Program.m_szLogFileName);
            if (!string.IsNullOrEmpty(foldername) && !Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }
            File.AppendAllText(Program.m_szLogFileName, appending);
        }
    }


}

