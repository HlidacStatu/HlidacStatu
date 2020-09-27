using System;
using System.Collections.Generic;
using System.Linq;
using Devmasters.Cache;

namespace HlidacStatu.Util.Cache
{

    public class AutoUpdateCouchbaseCacheManager<T, Key>
        : Manager<T,Key, AutoUpdateCouchbaseCache<T>>
        where T : class
    {

        string bucketName = null;
        public AutoUpdateCouchbaseCacheManager(string keyPrefix, System.Func<Key, T> func, TimeSpan expiration, string bucketName = null)
            : base(keyPrefix, func,expiration)
        {
            this.bucketName = bucketName;
        }
        protected override AutoUpdateCouchbaseCache<T> getTCacheInstance(Key key, TimeSpan expiration, Func<Key, T> contentFunc)
        {

            return new AutoUpdateCouchbaseCache<T>(expiration, this.keyPrefix + key.ToString(), (o) => contentFunc.Invoke(key),this.bucketName);
        }


        public static AutoUpdateCouchbaseCacheManager<T, Key> GetSafeInstance(string instanceName, System.Func<Key, T> func, TimeSpan expiration)
        {
            lock (instancesLock)
            {
                string instanceFullName = instanceName + "|" + typeof(T).ToString() + "|" + typeof(Key).ToString();

                if (!instances.ContainsKey(instanceFullName))
                {
                    instances[instanceFullName] = new AutoUpdateCouchbaseCacheManager<T, Key>(instanceName, func, expiration);
                }
                return (AutoUpdateCouchbaseCacheManager<T, Key>)instances[instanceFullName];
            }
        }



    }
}
