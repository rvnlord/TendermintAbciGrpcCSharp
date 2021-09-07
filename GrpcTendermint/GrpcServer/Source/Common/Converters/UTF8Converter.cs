using System.Text;

namespace GrpcServer.Source.Common.Converters
{
    public static class UTF8Converter
    {
        public static byte[] UTF8ToByteArray(this string str) => Encoding.UTF8.GetBytes(str);
        public static string ToUTF8String(this byte[] arr) => Encoding.UTF8.GetString(arr);
    }
}
