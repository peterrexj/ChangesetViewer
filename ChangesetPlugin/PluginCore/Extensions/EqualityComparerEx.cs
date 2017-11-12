using System;
using System.Collections.Generic;

namespace PluginCore.Extensions
{
    public static class EqualityComparerEx
    {
        public static IEqualityComparer<T> Create<T>(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            return new TEqualityComparer<T>(equals, getHashCode);
        }
    }

    public class TEqualityComparer<T> : IEqualityComparer<T>
    {
        Func<T, T, bool> _equals;
        Func<T, int> _getHashCode;
        public TEqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            _equals = equals;
            _getHashCode = getHashCode;
        }

        #region IEqualityComparer<T> Members

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            return _equals(x, y);
        }

        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            return _getHashCode(obj);
        }

        #endregion
    }
}
