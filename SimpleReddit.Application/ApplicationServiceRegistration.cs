using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleReddit.Application.Contracts;
using SimpleReddit.Application.Features;
using SimpleReddit.Application.Models;

namespace SimpleReddit.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RedditSetting>(configuration.GetSection("RedditSetting"));
            services.AddTransient<IRedditService, RedditService>();


            // Inject Cache service
            //services.AddSingleton<IEntitySerializer, EntitySerializer>();
            //services.AddSingleton<IDistributedCacheService, DistributedCacheService>();

            return services;
        }
    }
}