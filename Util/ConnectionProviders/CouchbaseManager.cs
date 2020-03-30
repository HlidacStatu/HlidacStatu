using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;

namespace HlidacStatu.Util.ConnectionProviders
{
    public class CouchbaseManager
    {
        private static object _clientLock = new object();
        private static Dictionary<string, IBucket> _clients = new Dictionary<string, IBucket>();

        private static object locker = new object();

        public static IBucket GetCouchbaseClient(string bucketName)
        {
            lock (_clientLock)
            {
                if (!_clients.ContainsKey(bucketName))
                {
                    Cluster cluster = new Cluster(new Couchbase.Configuration.Client.ClientConfiguration
                    {
                        Servers = ConfigurationManager.AppSettings["CouchbaseServers"].Split(',').Select(s => new Uri(s)).ToList()
                    });

                    var authenticator = new Couchbase.Authentication.PasswordAuthenticator(
                        ConfigurationManager.AppSettings["CouchbaseUsername"],
                        ConfigurationManager.AppSettings["CouchbasePassword"]);
                    cluster.Authenticate(authenticator);
                    var bucket = cluster.OpenBucket(bucketName);

                    _clients.Add(bucketName, bucket);
                }
                return _clients[bucketName];
            }

        }

    }
}
