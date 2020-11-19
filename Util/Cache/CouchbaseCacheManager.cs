using System;
using System.Collections.Generic;
using System.Linq;
using Devmasters.Cache;

namespace HlidacStatu.Util.Cache
{

    public class CouchbaseCacheManager<T, Key>
        : Manager<T,Key, CouchbaseCache<T>>
        where T : class
    {
        string bucketName = null;
        Func<Key, string> keyValueSelector = null;
        public CouchbaseCacheManager(string keyPrefix, System.Func<Key, T> func, TimeSpan expiration, string bucketName = null, Func<Key, string> keyValueSelector = null)
            : base(keyPrefix, func,expiration)
        {
            this.bucketName = bucketName;
            this.keyValueSelector = keyValueSelector ??  new Func<Key, string>(k=>k.ToString());
        }
        protected override CouchbaseCache<T> getTCacheInstance(Key key, TimeSpan expiration, Func<Key, T> contentFunc)
        {
            return new CouchbaseCache<T>(expiration, this.keyPrefix + this.keyValueSelector(key), (o) => contentFunc.Invoke(key),bucketName);
        }

        public static CouchbaseCacheManager<T, Key> GetSafeInstance(string instanceName, System.Func<Key, T> func, TimeSpan expiration, 
            Func<Key,string> keyValueSelector = null)
        {
            lock (instancesLock)
            {
                string instanceFullName = instanceName + "|" + typeof(T).ToString() + "|" + typeof(Key).ToString();

                if (!instances.ContainsKey(instanceFullName))
                {
                    instances[instanceFullName] = new CouchbaseCacheManager<T, Key>(instanceName, func, expiration, 
                        keyValueSelector: keyValueSelector);
                }
                return (CouchbaseCacheManager<T, Key>)instances[instanceFullName];
            }
        }



    }
}
