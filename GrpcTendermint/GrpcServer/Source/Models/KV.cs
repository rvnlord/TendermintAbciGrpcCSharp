using System;
using System.Collections.Generic;
using System.Linq;
using GrpcServer.Source.Common.Converters;
using LiteDB;

namespace GrpcServer.Source.Models
{
    public class KV
    {
        [BsonId]
        public string Key { get; set; }
        public string Value { get; set; }

        public override string ToString() => $"{Key.Base64ToUTF8()}={Value.Base64ToUTF8()}";
    }

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
