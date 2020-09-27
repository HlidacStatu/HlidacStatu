using Couchbase;
using System.Linq;
using Devmasters.Cache;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace HlidacStatu.Util.Cache
{
    [Serializable()]
    public class AutoUpdateCouchbaseCache<T> : AutoUpdatebleCache<T>
        where T : class
    {

        public AutoUpdateCouchbaseCache(TimeSpan expiration, System.Func<object, T> contentFunc, string bucketname = null)
            : this(expiration, null, contentFunc, null,bucketname)
        { }
        public AutoUpdateCouchbaseCache(TimeSpan expiration, string cacheKey, System.Func<object, T> contentFunc, string bucketname = null)
            : this(expiration, cacheKey, contentFunc, null,bucketname)
        { }
        public AutoUpdateCouchbaseCache(TimeSpan expiration, string cacheKey, System.Func<object, T> contentFunc, object parameters, string bucketname = null)
            : base(new CouchbaseCacheProvider<T>(bucketname), expiration, cacheKey, contentFunc, parameters)
        {
        }

    }

}