#if NET45
using System;

namespace Standard.IO.Compression.LZ4Encoding
{
    internal static class Polyfill
    {
        public static T[] Empty<T>(this Array array)
        {
            //Contract.Ensures(Contract.Result<T[]>() != null);
            //Contract.Ensures(Contract.Result<T[]>().Length == 0);
            //Contract.EndContractBlock();
            return new T[0];
        }
    }
}

#endif    
