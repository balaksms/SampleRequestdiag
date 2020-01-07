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
    class ErrMessage
    {
        public const string INFO = "INFO";
        public const string STATS = "STATS";
        public const string EXCEPTION = "EXCEPTION";
        public const string Error = "Error";
        public const string ENTER = "ENTER";
        public const string EXIT = "EXIT";
        public const string MsgCallstack = "Message {0,-20} - CallStack {1,-100}";
        public const string Logformat = "{0:d,-10} {1,-12} | {2 ,-50} | {3,-10} | {4,-50}";
        public const string LogFileInit = "Log File {0} initialized";
        public const string InitParamFailed = "Initializing Parameters Failed";
        public const string InitParamValue = "Paramater : {0} Value : {1}";
        public const string RequestDiagString = "Request Diagnostic String : {0}";
        public const string RequestLatencyStats = "Request Latency Stats : {0}";
        public const string RequestChargeStats = "Request Charge Stats: {0}";
        public const string RequestQueryMetrics = "Request Query Metric Stats: {0}";
    }

    class Logging
    {
        internal static void LogInfo(string sMsg, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            //format
            //01/01/2018 22:00:00:000  5 spaces Function Name (Length 20) 5 spaces Message
            string appending = String.Format(ErrMessage.Logformat, DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.ToLongTimeString(), memberName, ErrMessage.INFO, sMsg);
            appending = appending + string.Format("{0}", System.Environment.NewLine);
            string foldername = System.IO.Path.GetDirectoryName(Program.m_szLogFileName);
            if (!string.IsNullOrEmpty(foldername) && !Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }
            File.AppendAllText(Program.m_szLogFileName, appending);

        }
        internal static void LogEnter([System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            //format
            //01/01/2018 22:00:00:000  5 spaces Function Name (Length 20) 5 spaces Message
            string appending = String.Format(ErrMessage.Logformat, DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.ToLongTimeString(), memberName, ErrMessage.ENTER, memberName);
            appending = appending + string.Format("{0}", System.Environment.NewLine);
            string foldername = System.IO.Path.GetDirectoryName(Program.m_szLogFileName);
            if (!string.IsNullOrEmpty(foldername) && !Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }
            File.AppendAllText(Program.m_szLogFileName, appending);

        }
        internal static void LogExit([System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            //format
            //01/01/2018 22:00:00:000  5 spaces Function Name (Length 20) 5 spaces Message
            string appending = String.Format(ErrMessage.Logformat, DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.ToLongTimeString(), memberName, ErrMessage.EXIT, memberName);
            appending = appending + string.Format("{0}", System.Environment.NewLine);
            string foldername = System.IO.Path.GetDirectoryName(Program.m_szLogFileName);
            if (!string.IsNullOrEmpty(foldername) && !Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }
            File.AppendAllText(Program.m_szLogFileName, appending);

        }
        internal static void LogException(Exception ex, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            string appending;
            //format
            if (ex.InnerException != null)
            {
                string innerstacktrace = ex.InnerException.StackTrace != null ? ex.InnerException.StackTrace.ToString() : string.Empty;
                appending = String.Format(ErrMessage.Logformat, DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.ToLongTimeString(), memberName, ErrMessage.EXCEPTION, string.Format(ErrMessage.MsgCallstack, ex.InnerException.Message.ToString(), innerstacktrace));
                appending = appending + String.Format(ErrMessage.Logformat, DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.ToLongTimeString(), memberName, ErrMessage.EXCEPTION, string.Format(ErrMessage.MsgCallstack, ex.Message.ToString(), ex.StackTrace.ToString()));
            }
            else
            {
                appending = String.Format(ErrMessage.Logformat, DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.ToLongTimeString(), memberName, ErrMessage.EXCEPTION, string.Format(ErrMessage.MsgCallstack, ex.Message.ToString(), ex.StackTrace.ToString()));
            }


            appending = appending + string.Format("{0}", System.Environment.NewLine);
            string foldername = System.IO.Path.GetDirectoryName(Program.m_szLogFileName);
            if (!string.IsNullOrEmpty(foldername) && !Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }
            File.AppendAllText(Program.m_szLogFileName, appending);

        }
        internal static void LogStats(string stats, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            //format
            //01/01/2018 22:00:00:000  5 spaces Function Name (Length 20) 5 spaces Message
            string appending = String.Format(ErrMessage.Logformat, DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.ToLongTimeString(), memberName, ErrMessage.STATS, stats);
            appending = appending + string.Format("{0}", System.Environment.NewLine);
            string foldername = System.IO.Path.GetDirectoryName(Program.m_szLogFileName);
            if (!string.IsNullOrEmpty(foldername) && !Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }
            File.AppendAllText(Program.m_szLogFileName, appending);

        }

        internal static void LogError(string errInfo, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            string appending;
            //format
            if (errInfo != null)
            {

                appending = String.Format(ErrMessage.Logformat, DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.ToLongTimeString(), memberName, ErrMessage.Error, string.Format(ErrMessage.MsgCallstack, errInfo, Environment.StackTrace));
            }
            else
            {
                appending = String.Format(ErrMessage.Logformat, DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.ToLongTimeString(), memberName, ErrMessage.Error, string.Format(ErrMessage.MsgCallstack, errInfo, Environment.StackTrace));
            }


            appending = appending + string.Format("{0}", System.Environment.NewLine);
            string foldername = System.IO.Path.GetDirectoryName(Program.m_szLogFileName);
            if (!string.IsNullOrEmpty(foldername) && !Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }
            File.AppendAllText(Program.m_szLogFileName, appending);

        }

    }

    class Program
    {
        public static int m_maxdoccount = 100;
        public static string m_szLogFileName = "log\\DebugLog.txt";
        private static string m_szDatabaseName = "dbrequestdiagdemo";
        private static string m_szCollectionName = "crequestdiagdemo";
        private static string m_szAccountURL = "https://testmigrationtool.documents.azure.com:443/";
        private static string m_szAccountKey = "56QXkKOyhxgKCcdhe5ybT5VQYsjcoDJNHF8VgA5V7Mjtw2xOwAvCmzBqRHQUsdsQ26WlpRe6b6eBjiwH9FG2qw==";
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
                Logging.LogEnter();
                Logging.LogInfo(string.Format(ErrMessage.LogFileInit, m_szLogFileName));

                TestDocDB().Wait();
                CreateDoc().Wait();
                List<customer> ret = QueryCustomer().Result;
                ReadDoc(ret).Wait();
                ReplaceDoc(ret).Wait();
                UpsertDoc(ret).Wait();
                DeleteDoc(ret).Wait();
                Logging.LogExit();
            }
            catch (Exception ex)
            {
                Logging.LogException(ex);
                
                File.AppendAllText(m_szLogFileName, ex.Message.ToString());

                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message.ToString());
                    File.AppendAllText(m_szLogFileName, ex.InnerException.Message.ToString());
                }
                File.AppendAllText(m_szLogFileName, ex.StackTrace.ToString());


            }
        }
        private static async Task TestDocDB()
        {

            Logging.LogEnter();
            DocumentCollection m_coll=null;
            Console.SetWindowSize(100, 45);
            m_szDatabaseName = ConfigurationManager.AppSettings.Get("DatabaseName");
            m_szCollectionName = ConfigurationManager.AppSettings.Get("CollectionName");
            m_szAccountURL = ConfigurationManager.AppSettings.Get("AccountURL");
            m_szAccountKey = ConfigurationManager.AppSettings.Get("AccountKey");


            m_DC = new DocumentClient(new Uri(m_szAccountURL), m_szAccountKey, new ConnectionPolicy()


            {
                ConnectionMode = ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Tcp,
                //RetryOptions = new RetryOptions() { MaxRetryAttemptsOnThrottledRequests = 0 },
            });

            try
            {
                m_DC.OpenAsync().Wait();
            }
            catch (Exception ex)
            {

                Logging.LogException(ex);
                return;
            }

            DocumentCollection myCollection = new DocumentCollection();
            myCollection.Id = m_szCollectionName;
            myCollection.PartitionKey.Paths.Add("/Region");

            Database database = await m_DC.CreateDatabaseIfNotExistsAsync(new Database { Id = m_szDatabaseName });

            try
            { 
             m_coll= await m_DC.CreateDocumentCollectionIfNotExistsAsync(
                database.SelfLink,
                myCollection,
                new RequestOptions { OfferThroughput = 400 });
            }
            catch( DocumentClientException ex)
            {
                Logging.LogException(ex);
            }

            Logging.LogExit();
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
                    Logging.LogStats(string.Format(ErrMessage.RequestDiagString, cxResponse.RequestDiagnosticsString.ToString()));
                    Logging.LogStats(string.Format(ErrMessage.RequestLatencyStats, cxResponse.RequestLatency.ToString()));
                    Logging.LogStats(string.Format(ErrMessage.RequestChargeStats, cxResponse.RequestCharge.ToString()));
                }

            }
            catch (DocumentClientException dex) 
            {
                Logging.LogException(dex);
            }
            catch(Exception ex)
            {
                Logging.LogException(ex);
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
                    Logging.LogStats(string.Format(ErrMessage.RequestDiagString, cxResponse.RequestDiagnosticsString.ToString()));
                    Logging.LogStats(string.Format(ErrMessage.RequestLatencyStats, cxResponse.RequestLatency.ToString()));
                    Logging.LogStats(string.Format(ErrMessage.RequestChargeStats, cxResponse.RequestCharge.ToString()));
                }
            }
            catch (DocumentClientException dex)
            {

                Logging.LogException(dex);
            }
            catch(Exception ex)
            {
                Logging.LogException(ex);
            }
        }
        private static async Task ReadDoc(List<customer> pitem)
        {
            try
            {
                foreach(customer citem in pitem)
                { 
                ResourceResponse<Document> cxResponse = await m_DC.ReadDocumentAsync(UriFactory.CreateDocumentUri(m_szDatabaseName, m_szCollectionName, citem.Id).ToString(), new RequestOptions { PartitionKey = new PartitionKey(citem.Region) });
                Logging.LogStats(string.Format(ErrMessage.RequestDiagString, cxResponse.RequestDiagnosticsString.ToString()));
                Logging.LogStats(string.Format(ErrMessage.RequestLatencyStats, cxResponse.RequestLatency.ToString()));
                Logging.LogStats(string.Format(ErrMessage.RequestChargeStats, cxResponse.RequestCharge.ToString()));
                }
            }
            catch (DocumentClientException dex)
            {

                Logging.LogException(dex);
            }
            catch (Exception ex)
            {
                Logging.LogException(ex);
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
                    Logging.LogStats(ErrMessage.RequestDiagString, result.RequestDiagnosticsString);
                    Logging.LogStats(ErrMessage.RequestQueryMetrics, result.QueryMetrics.ToString());
                    Logging.LogStats(ErrMessage.RequestChargeStats, result.RequestCharge.ToString());
                    output.AddRange(result.ToList());
                }
            }
            catch(DocumentClientException dex)
            {
                Logging.LogException(dex);
            }
            catch(Exception ex)
            {
                Logging.LogException(ex);
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
                    Logging.LogStats(string.Format(ErrMessage.RequestDiagString, response.RequestDiagnosticsString.ToString()));
                    Logging.LogStats(string.Format(ErrMessage.RequestLatencyStats, response.RequestLatency.ToString()));
                    Logging.LogStats(string.Format(ErrMessage.RequestChargeStats, response.RequestCharge.ToString()));
                }
                catch(DocumentClientException dex)
                {
                    Logging.LogException(dex);
                }
                catch(Exception ex)
                {
                    Logging.LogException(ex);
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
                Logging.LogStats(string.Format(ErrMessage.RequestDiagString, response.RequestDiagnosticsString.ToString()));
                Logging.LogStats(string.Format(ErrMessage.RequestLatencyStats, response.RequestLatency.ToString()));
                Logging.LogStats(string.Format(ErrMessage.RequestChargeStats, response.RequestCharge.ToString()));
                }
                catch(DocumentClientException dex)
                {
                    Logging.LogException(dex);
                }
                catch(Exception ex)
                {
                    Logging.LogException(ex);
                }
            }

        }
    }


}

