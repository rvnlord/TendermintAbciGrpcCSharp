using System.Collections.Concurrent;
using GrpcServer.Source.Models;

namespace GrpcServer.Source.Services
{
    public interface IKVStoreCacheService
    {
        ConcurrentDictionary<string, KV> KVs { get; set; }
    }
}
