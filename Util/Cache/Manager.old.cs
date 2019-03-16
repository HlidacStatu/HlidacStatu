using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Util.Cache
{

    public class REMOVE_Manager<T, Key, TCache>
        where T : class
        where TCache : Devmasters.Cache.V20.BaseCache<T>
    {
        protected System.Func<Key, T> contentFunc = null;
        protected TimeSpan expiration = TimeSpan.Zero;
        protected string keyPrefix = null;
        protected object lockObj = new object();
        protected Dictionary<Key, TCache> cacheArray = new Dictionary<Key, TCache>();

        public REMOVE_Manager(string keyprefix, System.Func<Key, T> func, TimeSpan expiration)
        {
            this.contentFunc = func;
            this.expiration = expiration;
            this.keyPrefix = keyprefix + "#";
        }

        public virtual T Get(Key key)
        {
            return Get(key, this.expiration);
        }
        public virtual T Get(Key key, TimeSpan expiration)
        {
            lock (lockObj)
            {
                if (cacheArray.ContainsKey(key))
                {
                    var val = cacheArray[key].Get();
                    if (val != null)
                        return val;
                }

                T value = contentFunc.Invoke(key);
                if (value != null)
                {
                    cacheArray.Add(key, getTCacheInstance(key, expiration, o=> contentFunc.Invoke(key))
                    //new TCache(expiration, prefix + key.ToString(), (obj) => contentFunc.Invoke(key))
                    );
                    cacheArray[key].ForceRefreshCache(value);
                }
                return value;
            }
        }

        protected virtual TCache getTCacheInstance(Key key, TimeSpan expiration, System.Func<Key, T> contentFunc)
        {
            throw new NotImplementedException();
        }

        public void Delete(Key key)
        {
            this.Set(key, null, this.expiration);
        }

        public void Set(Key key, T obj)
        {
            this.Set(key, obj, this.expiration);
        }

        public void Set(Key key, T obj, TimeSpan expiration)
        {
            if (obj == null && cacheArray.ContainsKey(key))
                cacheArray[key].Invalidate();
            else
            {
                if (!cacheArray.ContainsKey(key))
                {
                    cacheArray.Add(key,
                        getTCacheInstance(key, expiration, o => contentFunc.Invoke(key))
                        //new Devmasters.Cache.V20.LocalMemory.LocalMemoryCache<T>(expiration, this.prefix + key.ToString(), (o) => contentFunc.Invoke(key))
                        );
                }

                cacheArray[key].ForceRefreshCache(obj);
            }
        }

        public Key[] Keys()
        {
            return cacheArray.Keys.ToArray();
        }


        protected static object instancesLock = new object();
        protected static Dictionary<string, REMOVE_Manager<T, Key,TCache>> instances = new Dictionary<string, REMOVE_Manager<T, Key, TCache>>();



    }
}
