using System.Linq;

namespace GrpcServer.Source.Common.Extensions
{
    public static class TExtensions
    {
        public static bool In<T>(this T o, params T[] os) => os.Length > 0 && os.Any(s => s.Equals(o));
    }
}
