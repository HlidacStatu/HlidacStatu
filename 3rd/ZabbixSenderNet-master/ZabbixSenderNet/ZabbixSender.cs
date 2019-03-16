using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ysq.Zabbix
{
    public class Sender
    {
        private int _port;
        private string _zabbixServer;

        public Sender(string zabbixServer, int port = 10051)
        {
            _zabbixServer = zabbixServer;
            _port = port;
        }

        public SenderResponse Send(string host, string itemKey, string value, int timeout = 500)
        {
            dynamic req = new ExpandoObject();
            req.request = "sender data";
            dynamic d = new ExpandoObject();
            d.host = host;
            d.key = itemKey;
            d.value = value;
            req.data = new[] { d };
            var jsonReq = JsonConvert.SerializeObject(req);
            using (var tcpClient = new TcpClient(_zabbixServer, _port))
            using(var networkStream = tcpClient.GetStream())
            {
                var data = Encoding.ASCII.GetBytes(jsonReq);
                networkStream.Write(data, 0, data.Length);
                networkStream.Flush();
                var counter = 0;
                while(!networkStream.DataAvailable)
                {
                    if (counter < timeout/50)
                    {
                        counter++;
                        Thread.Sleep(50);
                    }
                    else
                    {
                        throw new TimeoutException();
                    }
                }

                var resbytes = new Byte[1024];
                networkStream.Read(resbytes, 0, resbytes.Length);
                var s = Encoding.UTF8.GetString(resbytes);
                var jsonRes = s.Substring(s.IndexOf('{'));
                return JsonConvert.DeserializeObject<SenderResponse>(jsonRes);
            }       
        }
    }
}