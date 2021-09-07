using System;

namespace GrpcServer.Source.Common.Converters
{
    public static class Base64Converter
    {
        public static byte[] Base64ToByteArray(this string str) => Convert.FromBase64String(str);
        public static string ToBase64String(this byte[] arr) => Convert.ToBase64String(arr);
        public static string UTF8ToBase64(this string utf8str) => utf8str.UTF8ToByteArray().ToBase64String();
        public static string Base64ToUTF8(this string base64) => base64.Base64ToByteArray().ToUTF8String();
    }
}
