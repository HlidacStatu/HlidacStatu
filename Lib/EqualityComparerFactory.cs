using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib
{
    public static class EqualityComparerFactory<T>
    {
        private class MyComparer : IEqualityComparer<T>
        {
            private readonly Func<T, int> _getHashCodeFunc;
            private readonly Func<T, T, bool> _equalsFunc;

            public MyComparer(Func<T, int> getHashCodeFunc, Func<T, T, bool> equalsFunc)
            {
                _getHashCodeFunc = getHashCodeFunc;
                _equalsFunc = equalsFunc;
            }

            public bool Equals(T x, T y)
            {
                return _equalsFunc(x, y);
            }

            public int GetHashCode(T obj)
            {
                return _getHashCodeFunc(obj);
            }
        }

        public static IEqualityComparer<T> CreateComparer(Func<T, int> getHashCodeFunc, Func<T, T, bool> equalsFunc)
        {
            if (getHashCodeFunc == null)
                throw new ArgumentNullException("getHashCodeFunc");
            if (equalsFunc == null)
                throw new ArgumentNullException("equalsFunc");

            return new MyComparer(getHashCodeFunc, equalsFunc);
        }
    }

    public class GenericCompare<T> : IEqualityComparer<T> where T : class
    {
        private Func<T, object> _expr { get; set; }
        public GenericCompare(Func<T, object> expr)
        {
            this._expr = expr;
        }
        public bool Equals(T x, T y)
        {
            var first = _expr.Invoke(x);
            var sec = _expr.Invoke(y);
            if (first != null && first.Equals(sec))
                return true;
            else
                return false;
        }
        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
