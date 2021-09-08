using GrpcServer.Source.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcServer.Source.Common.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddKVStoreCache(this IServiceCollection services) => services.AddSingleton<IKVStoreCacheService, KVStoreCacheService>();
    }
}
