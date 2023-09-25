using SimpleReddit.Cache.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleReddit.Cache
{
    public static class CacheServiceRegistration
    {
        public static IServiceCollection AddCacheServices(this IServiceCollection services)
        {
            // Inject Cache service
            services.AddSingleton<IDistributedCacheService, DistributedCacheService>();

            return services;
        }
    }
}
