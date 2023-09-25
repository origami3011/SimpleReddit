using Microsoft.OpenApi.Models;
using SimpleReddit.Api.Contracts;
//using SimpleReddit.Api.Models;
using SimpleReddit.Api.Services;
using SimpleReddit.Cache;
using SimpleReddit.Cache.Interfaces;
using SimpleReddit.FileService;
using SimpleReddit.FileService.Contracts;
using SimpleReddit.Models;
//using SimpleReddit.Application;

namespace SimpleReddit.Api
{
    public static class StartupExtension
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            AddSwagger(builder.Services);   //  add swagger document for API

            builder.Services.Configure<RedditSetting>(builder.Configuration.GetSection("RedditSetting"));
            
            // start custom DI
            builder.Services.AddTransient<IRedditService, RedditService>();            
            builder.Services.AddSingleton<IEntitySerializer, EntitySerializer>();
            builder.Services.AddSingleton<IDistributedCacheService, DistributedCacheService>();
            builder.Services.AddSingleton<ITokenService, TokenService>();
            // end custom DI

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddEndpointsApiExplorer();            
            builder.Services.AddControllers();

            builder.Services.AddHttpClient("example", client =>
            {
                client.BaseAddress = new Uri("https://api.example.com/");
                // Add any other configuration you need for this HttpClient
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            return builder.Build();
        }
        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            return app;
        }

        public static void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {                
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Reddit API"
                });
            });
        }
    }
}
