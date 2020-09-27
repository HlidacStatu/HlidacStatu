using System;

namespace HlidacStatu.Util.Cache
{
    public struct KeyAndId
    {
        public string CacheNameOnDisk { get; set; }
        public string ValueForData { get; set; }
    }

    public class FileCacheManager
        : Manager<byte[], KeyAndId, Devmasters.Cache.File.BinaryFileCache>
    {

        public FileCacheManager(string keyPrefix, System.Func<KeyAndId, byte[]> func, TimeSpan expiration)
            : base(keyPrefix, func, expiration)
        {
        }

        
        protected override Devmasters.Cache.File.BinaryFileCache getTCacheInstance(KeyAndId key, TimeSpan expiration, Func<KeyAndId, byte[]> contentFunc)
        {
            return new Devmasters.Cache.File.BinaryFileCache(
                Devmasters.Config.GetWebConfigValue("FileCachePath"),
                expiration, key.CacheNameOnDisk,
                (o) => contentFunc.Invoke(key)
                );

            // return new Devmasters.Cache.LocalMemory.LocalMemoryCache<T>(expiration, this.keyPrefix + key.ToString(), (o) => contentFunc.Invoke(key));

        }

        private static FileCacheManager GetSafeInstance(Type instanceType)
        {
            lock (instancesLock)
            {
                string instanceFullName = instanceType.AssemblyQualifiedName + "|" + typeof(byte[]).ToString() + "|" + typeof(string).ToString();
                if (!instances.ContainsKey(instanceFullName))
                {
                    instances[instanceFullName] = (FileCacheManager)Activator.CreateInstance(instanceType);
                }
                return (FileCacheManager)instances[instanceFullName];
            }
        }
        public static FileCacheManager GetSafeInstance(string instanceName, System.Func<KeyAndId, byte[]> func, TimeSpan expiration)
        {
            lock (instancesLock)
            {
                string instanceFullName = instanceName + "|" + typeof(byte[]).ToString() + "|" + typeof(string).ToString();

                if (!instances.ContainsKey(instanceFullName))
                {
                    instances[instanceFullName] = new FileCacheManager(instanceName, func, expiration);
                }
                return (FileCacheManager)instances[instanceFullName];
            }
        }


    }
}
