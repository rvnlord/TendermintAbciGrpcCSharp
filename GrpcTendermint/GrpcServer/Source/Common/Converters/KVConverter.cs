using System;
using System.Collections.Generic;
using System.Linq;
using GrpcServer.Source.Models;

namespace GrpcServer.Source.Common.Converters
{
    public static class KVConverter
    {
        public static KV ToKV(this IEnumerable<string> en)
        {
            var arr = en.ToArray();
            if (arr.Length > 2)
                throw new ArgumentOutOfRangeException(nameof(arr), "Outer enumerable must contain exactly 2 elements");
            return new KV { Key = arr.Length > 0 ? arr[0] : default, Value = arr.Length > 1 ? arr[1] : default };
        }
    }
}
