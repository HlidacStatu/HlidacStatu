using System;
using System.Collections.Generic;
using System.Linq;
using Devmasters.Cache;

namespace HlidacStatu.Util.Cache
{

    public class CouchbaseCacheManager<T, Key>
        : Manager<T,Key, Devmasters.Cache.Couchbase.CouchbaseCache<T>>
        where T : class
    {
        private string bucketName = "";
        private string username = "";
        private string password = "";
        private string[] serversUrl;
        Func<Key, string> keyValueSelector = null;
        public CouchbaseCacheManager(string keyPrefix, System.Func<Key, T> func, TimeSpan expiration
            , string[] serversUrl, string couchbaseBucketName, string username, string password
            , Func<Key, string> keyValueSelector = null)
            : base(keyPrefix, func,expiration)
        {
            this.bucketName = couchbaseBucketName;
            this.serversUrl = serversUrl;
            this.username = username;
            this.password = password;
            this.keyValueSelector = keyValueSelector ??  new Func<Key, string>(k=>k.ToString());
        }
        protected override Devmasters.Cache.Couchbase.CouchbaseCache<T> getTCacheInstance(Key key, TimeSpan expiration, Func<Key, T> contentFunc)
        {
            return new Devmasters.Cache.Couchbase.CouchbaseCache<T>(expiration, this.keyPrefix + this.keyValueSelector(key), (o) => contentFunc.Invoke(key),
                                this.serversUrl, this.bucketName, this.username, this.password);
        }

        public static CouchbaseCacheManager<T, Key> GetSafeInstance(string instanceName, System.Func<Key, T> func, TimeSpan expiration,
            string[] serversUrl, string couchbaseBucketName, string username, string password,
            Func<Key,string> keyValueSelector = null)
        {
            lock (instancesLock)
            {
                string instanceFullName = instanceName + "|" + typeof(T).ToString() + "|" + typeof(Key).ToString();

                if (!instances.ContainsKey(instanceFullName))
                {
                    instances[instanceFullName] = new CouchbaseCacheManager<T, Key>(instanceName, func, expiration,
                        serversUrl, couchbaseBucketName, username, password,
                        keyValueSelector: keyValueSelector);
                }
                return (CouchbaseCacheManager<T, Key>)instances[instanceFullName];
            }
        }



    }
}
