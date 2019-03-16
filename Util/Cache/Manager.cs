using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Util.Cache
{

    public class Manager<T, Key, TCache>
        where T : class
        where TCache : Devmasters.Cache.V20.BaseCache<T>
    {
        protected System.Func<Key, T> contentFunc = null;
        protected TimeSpan expiration = TimeSpan.Zero;
        protected string keyPrefix = null;
        protected object lockObj = new object();

        public Manager(string keyprefix, System.Func<Key, T> func, TimeSpan expiration)
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
            return getTCacheInstance(key, expiration, o => contentFunc.Invoke(key)).Get();                
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
            if (obj == null)
                getTCacheInstance(key, expiration, o => contentFunc.Invoke(key)).Invalidate();
            else
            {
                getTCacheInstance(key, expiration, o => contentFunc.Invoke(key)).ForceRefreshCache(obj);
            }
        }


        protected static object instancesLock = new object();
        protected static Dictionary<string, Manager<T, Key,TCache>> instances = new Dictionary<string, Manager<T, Key, TCache>>();



    }
}
