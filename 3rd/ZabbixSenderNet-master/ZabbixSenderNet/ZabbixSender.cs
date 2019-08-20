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

        public SenderResponse Send(string host, string itemKey, string value, int timeout = 2500)
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
                byte[] zabxHeader = Encoding.ASCII.GetBytes("ZBXD");
                var data = Encoding.ASCII.GetBytes(jsonReq);

                byte[] header = new byte[] {
                    zabxHeader[0],zabxHeader[1],zabxHeader[2],zabxHeader[3], 1,
                    (byte)(data.Length & 0xFF),
                    (byte)((data.Length >> 8) & 0xFF),
                    (byte)((data.Length >> 16) & 0xFF),
                    (byte)((data.Length >> 24) & 0xFF),
                    0, 0, 0, 0};

                //byte[] finalDataArray = new byte[header.Length + data.Length];

                //System.Buffer.BlockCopy(header, 0, finalDataArray, 0, header.Length);
                //System.Buffer.BlockCopy(data, 0, finalDataArray, header.Length, data.Length);

                networkStream.Write(header, 0, header.Length);
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