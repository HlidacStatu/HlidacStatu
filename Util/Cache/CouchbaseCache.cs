using Devmasters.Cache;
using System;
using System.Collections.Generic;

namespace HlidacStatu.Util.Cache
{

    [Serializable()]
    public class CouchbaseCache<T> : BaseCache<T>
        where T : class
    {

        public CouchbaseCache(TimeSpan expiration, System.Func<object, T> contentFunc, string bucketname = null)
            : this(expiration, null, contentFunc, null,bucketname)
        { }
        public CouchbaseCache(TimeSpan expiration, string cacheKey, System.Func<object, T> contentFunc, string bucketname = null)
            : this(expiration, cacheKey, contentFunc, null, bucketname)
        { }
        public CouchbaseCache(TimeSpan expiration, string cacheKey, System.Func<object, T> contentFunc, object parameters, string bucketname = null)
            : base(new CouchbaseCacheProvider<T>(bucketname), expiration, cacheKey, contentFunc, parameters)
        {
        }

    }

}