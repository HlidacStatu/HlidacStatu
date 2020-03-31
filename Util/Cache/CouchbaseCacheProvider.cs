using Couchbase;
using System.Linq;
using Devmasters.Cache.V20;
using System;
using System.Configuration;

namespace HlidacStatu.Util.Cache
{
    public class CouchbaseCacheProvider<T> : ICacheProvider<T>
      where T : class
    {
        private Couchbase.Core.IBucket bucketConn { get;  set; } = null;

        bool randomizeExpiration = true;

        public CouchbaseCacheProvider(bool randomizeExpiration)
            :this()
        {
            this.randomizeExpiration = randomizeExpiration;
        }

        private string couchbaseBucket = "";
        public CouchbaseCacheProvider()
            : this(null)
        {
        }
        public CouchbaseCacheProvider(string couchbaseBucketName)
        {
            this.couchbaseBucket = couchbaseBucketName ?? ConfigurationManager.AppSettings["CouchbaseBucket"];
        }

        public void Init()
        {
            if (bucketConn == null)
            {
                bucketConn = ConnectionProviders.CouchbaseManager.GetCouchbaseClient(this.couchbaseBucket);
            }
        }

        private string fixKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            if (key.Length > 250)
                return Devmasters.Core.CryptoLib.Hash.ComputeHash(key);
            else
                return key;
        }
        public void Remove(string key)
        {
            bucketConn.Remove(fixKey(key));
        }

        public void Insert(string key, T value, TimeSpan expiration)
        {
            if (randomizeExpiration && expiration != TimeSpan.Zero)
            {
                double percent10 = expiration.TotalSeconds * 0.1D;
                double change = (percent10 / 2D) - (Util.Consts.Rnd.NextDouble()*percent10);
                expiration = expiration.Add(TimeSpan.FromSeconds(change));
            }
            if (value != null)
            {
                if (expiration == TimeSpan.Zero)
                    bucketConn.Insert<T>(fixKey(key), value, TimeSpan.FromDays(365 * 2));
                else
                    bucketConn.Insert<T>(fixKey(key), value, expiration);
            }
            else
                BaseCache<T>.Logger.Warning(new Devmasters.Core.Logging.LogMessage()
                    .SetMessage("CouchbaseCacheProvider> null value")
                    .SetLevel(Devmasters.Core.Logging.PriorityLevel.Warning)
                    .SetCustomKeyValue("objectType", typeof(T).ToString())
                    .SetCustomKeyValue("cache key", key)
                    );

        }


        public bool Exists(string key)
        {
            return bucketConn.Exists(fixKey(key));
        }


        public T Get(string key)
        {

            IOperationResult<T> x = bucketConn.Get<T>(fixKey(key));
            if (x.Status == Couchbase.IO.ResponseStatus.KeyNotFound)
                return default(T);

            if (x.Status == Couchbase.IO.ResponseStatus.ClientFailure || x.Exception != null)
            {
                System.Threading.Thread.Sleep(10);
                x = bucketConn.Get<T>(fixKey(key));
                if (x.Status == Couchbase.IO.ResponseStatus.KeyNotFound)
                    return default(T);

                if (x.Status == Couchbase.IO.ResponseStatus.ClientFailure || x.Exception != null)
                {
                    throw new Couchbase.CouchbaseResponseException(x.Status.ToString(), x.Exception);
                }
            }
            return x.Value;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }


                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~LocalMemoryCacheProvider() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

}