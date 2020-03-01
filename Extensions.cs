using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net.Http;
using consulService.DTOs;
using consulService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Z.EntityFramework.Extensions;


namespace consulService.Config
{
    public static class Extensions
    {
        public static int ToInt(this string obj) {
            int ret = 0;

            ret = int.Parse(obj);

            return ret;
        }
        private static void AddDefaultServices(this IServiceCollection services) {
            services.AddDbContext<consulService.Memory.Config>(options => options.UseInMemoryDatabase(databaseName: "ConsulConfig"));
        }

        public static void UseVault(this IServiceCollection services, string[] key) {
            services.AddDefaultServices();

                        IServiceProvider provider = services.BuildServiceProvider();

            var loggerFactory = LoggerFactory.Create(builder =>
                        {
                            builder
                                .AddConsole()
                                .AddDebug();
            });
            

            ILogger<ConsulService> logger = loggerFactory.CreateLogger<ConsulService>();

            IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
            IHttpClientFactory httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();

            var service = new ConsulService(logger, httpClientFactory, configuration);

            List<Consul> list = new List<Consul>();
            var db = provider.GetRequiredService<consulService.Memory.Config>();

            foreach (var item in key)
            {
                var load = db.Get(item);
                if (load == null || (load.validUntil<DateTime.Now)) {
                    var save = service.GetFromConsul(item).Result;
                    if (save != null) {
                        db.Set(save);
                    }
                }
            }
        }
    }
}