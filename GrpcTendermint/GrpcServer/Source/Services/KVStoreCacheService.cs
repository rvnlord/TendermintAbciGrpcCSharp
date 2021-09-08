using System.Collections.Concurrent;
using GrpcServer.Source.Models;

namespace GrpcServer.Source.Services
{
    public class KVStoreCacheService : IKVStoreCacheService
    {
        public ConcurrentDictionary<string, KV> KVs { get; set; } = new();
    }
}
