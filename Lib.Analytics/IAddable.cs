using System.Collections.Generic;

namespace HlidacStatu.Lib.Analytics
{
    public interface IAddable<T>
    {
        T Add(T other);
    }
}
