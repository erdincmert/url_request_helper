using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Url_Request_Helper
{
    class Program
    {
        public static string ServerName { get; set; }
        public static string ModelName { get; set; }
        public static string UserName { get; set; }
        public static string Password { get; set; }
        private static string URL { get; set; }
        private static string DestinationTableName { get; set; }

        static async Task Main(string[] args)
        {
            ReadConsoleParameters(args);

            var clientHelper = new HttpClientHelper();
            var responses = await clientHelper.GetRequest(URL);

            DatabaseWriter<string> writer = new DatabaseWriter<string>();
            writer.SetConnectionString(ServerName, ModelName, UserName, Password);
            writer.WriteToDatabase(responses, DestinationTableName);
        }
        public static void ReadConsoleParameters(string[] args)
        {
            ServerName = args[0];
            ModelName = args[1];
            UserName = args[2];
            Password = args[3];
            URL = args[4];
            DestinationTableName = args[5];
        }
    }
}
