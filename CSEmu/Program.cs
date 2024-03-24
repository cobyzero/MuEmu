using CSEmu.Network;
using CSEmu.Network.Services;
using Serilog;
using System;
using System.Net;
using WebZen.Handlers;
using WebZen.Network;
using System.Linq;
using System.Threading;
 using System.Collections.Generic;
using CSEmu.XML;
using System.ComponentModel;

namespace CSEmu
{
    class Program
    {
        public static WZConnectServer server;
        public static WZChatServer WZChatServer;
        public static ClientManager Clients;
        private static readonly Dictionary<int, int> _classSuperior = new Dictionary<int, int>
        {
            { 1, 1 },
            { 5, 2 },
            { 10, 3 },
            { 30, 4 },
            { 50, 5 },
            { 100, 6 },
            { 200, 7 },
            { 300, 8 },
            { 9999, 9 },
        };
        private static readonly Dictionary<int, int> _class = new Dictionary<int, int>
        {
            { 6000, 10 },
            { 3000, 11 },
            { 1500, 12 },
            { 500, 13 },
            { 0, 14 }
        };
        static void Main(string[] args)
        {
            Serilog.Core.Logger logger = new LoggerConfiguration()
                .Destructure.ByTransforming<IPEndPoint>(endPoint => endPoint.ToString())
                .Destructure.ByTransforming<EndPoint>(endPoint => endPoint.ToString())
                .WriteTo.File("ConnectServer.txt")
                .WriteTo.Console(outputTemplate: "[{Level} {SourceContext}] {Message}{NewLine}{Exception}")
                .MinimumLevel.Debug()
                .CreateLogger();
            Log.Logger = logger;

            CSConfigDto xml;

            try
            {
                xml = XmlManagement.XmlLoader<CSConfigDto>("configuration.xml");
            }catch(Exception)
            {
                xml = fillFile(typeof(CSConfigDto), "xml") as CSConfigDto;
                XmlManagement.XmlSaver("configuration.xml", xml);
            }

            var mh = new MessageHandler[] {
                new FilteredMessageHandler<CSSession>()
                    .AddHandler(new MainServices())
            };

            var mf = new MessageFactory[]
            {
                new MainMessageFactory()
            };

            logger.Information("API Key for GameServers is {0}", xml.apiKey);
          

            var Connip = new IPEndPoint(IPAddress.Parse(xml.IP), 44405);
            var Chatip = new IPEndPoint(IPAddress.Parse(xml.IPChat), 55980);
            server = new WZConnectServer(Connip, mh, mf, false);
            WZChatServer = new WZChatServer(Chatip, mh, mf, false);
            ServerManager.Initialize(xml.apiKey);
            Clients = new ClientManager();

          

            while (true)
            {
                var input = Console.ReadLine();
                if (input == null)
                    break;

                if (input.Equals("exit", StringComparison.InvariantCultureIgnoreCase) ||
                    input.Equals("quit", StringComparison.InvariantCultureIgnoreCase) ||
                    input.Equals("stop", StringComparison.InvariantCultureIgnoreCase))
                    break;
            }
             
        }

        public static object fillFile(Type tType, string propParent = "")
        {
            //var tType = typeof(T);
            var file = Activator.CreateInstance(tType);

            var membs = tType.GetProperties();
            foreach(var mem in membs)
            {
                var prop = tType.GetProperty(mem.Name);
                if(prop.PropertyType.IsClass && typeof(string) != prop.PropertyType)
                {
                    prop.SetValue(file, fillFile(prop.PropertyType, propParent + "." + prop.Name));
                    continue;
                }
                var defaultv = prop.GetValue(file);
                var converter = TypeDescriptor.GetConverter(prop.PropertyType);
                Log.Information("Set Value for {0} type {1}, Default value {2}", propParent+"."+mem.Name, prop.PropertyType.Name, defaultv);
                Log.Information("Clear for default");
                var read = Console.ReadLine();
                if (!string.IsNullOrEmpty(read))
                    prop.SetValue(file, converter.ConvertFrom(read));
            }

            return file;
        }
         
    }
}
