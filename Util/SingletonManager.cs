using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public class SingletonManager<T, Key>
                where T : new()
    {


        Dictionary<Key, T> _instances = new Dictionary<Key, T>();
        object _instancesLock = new object();
        public T GetInstance(Key key)
        {
            lock (_instancesLock)
            {
                if (!_instances.ContainsKey(key))
                {
                    T inst = CreateInstance(key);
                    _instances.Add(key, inst);
                }

                return _instances[key];
            }
        }

        protected virtual T CreateInstance(Key key)
        {
            return new T();
        }

    }

    public class SingletonManagerWithSetup<T, Key>
        : SingletonManager<T,Key>
        where T : SingletonManagerWithSetup<T, Key>.ISetupableClass, new()
    {
        public interface ISetupableClass
        {
            void Setup(Key key);
        }


        Dictionary<Key, T> _instances = new Dictionary<Key, T>();
        object _instancesLock = new object();

        protected override T CreateInstance(Key key)
        {
            T inst = base.CreateInstance(key);
            inst.Setup(key);
            return inst;
        }
    }
}
