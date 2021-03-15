using System;
using System.Collections.Generic;
using System.Linq;
using Devmasters.Cache.LocalMemory;

namespace HlidacStatu.Util.Cache
{

    public class MemoryCacheManager<T, Key>
        : Manager<T,Key, Devmasters.Cache.LocalMemory.LocalMemoryCache<T>>
        where T : class
    {

        Func<Key, string> keyValueSelector = null;

        public MemoryCacheManager(string keyPrefix, System.Func<Key, T> func, TimeSpan expiration, Func<Key, string> keyValueSelector = null)
            : base(keyPrefix, func,expiration)
        {
            this.keyValueSelector = keyValueSelector ?? new Func<Key, string>(k => k.ToString());
        }
        protected override LocalMemoryCache<T> getTCacheInstance(Key key, TimeSpan expiration, Func<Key, T> contentFunc)
        {
            return new Devmasters.Cache.LocalMemory.LocalMemoryCache<T>(expiration, this.keyPrefix + this.keyValueSelector(key), (o) => contentFunc.Invoke(key));
        }

        private static MemoryCacheManager<T, Key> GetSafeInstance(Type instanceType)
        {
            lock (instancesLock)
            {
                string instanceFullName = instanceType.AssemblyQualifiedName + "|" + typeof(T).ToString() + "|" + typeof(Key).ToString();
                if (!instances.ContainsKey(instanceFullName))
                {
                    instances[instanceFullName] = (MemoryCacheManager<T, Key>)Activator.CreateInstance(instanceType);
                }
                return (MemoryCacheManager<T, Key>)instances[instanceFullName];
            }
        }
        public static MemoryCacheManager<T, Key> GetSafeInstance(string instanceName, System.Func<Key, T> func, TimeSpan expiration,
            Func<Key, string> keyValueSelector = null)
        {
            lock (instancesLock)
            {
                string instanceFullName = instanceName + "|" + typeof(T).ToString() + "|" + typeof(Key).ToString();

                if (!instances.ContainsKey(instanceFullName))
                {
                    instances[instanceFullName] = new MemoryCacheManager<T, Key>(instanceName, func, expiration, 
                        keyValueSelector: keyValueSelector);
                }
                return (MemoryCacheManager<T, Key>)instances[instanceFullName];
            }
        }



    }
}
