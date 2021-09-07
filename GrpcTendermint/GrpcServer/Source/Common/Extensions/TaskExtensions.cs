using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace GrpcServer.Source.Common.Extensions
{
    public static class TaskExtensions
    {
        public static Task<T> LogAsync<T, TL>(this Task<T> t, ILogger<TL> logger, string message, LogLevel logLevel = LogLevel.Information)
        {
            logger.Log(logLevel, message);
            return t;
        }
    }
}
