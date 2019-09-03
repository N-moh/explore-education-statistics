﻿using System.IO;
using AutoMapper;
using GovUk.Education.ExploreEducationStatistics.Data.Seed.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GovUk.Education.ExploreEducationStatistics.Data.Seed
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogDebug("Starting application");

            var seedService = serviceProvider.GetService<ISeedService>();
            seedService.Seed();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            services.AddMemoryCache();
            services.AddAutoMapper(typeof(Program).Assembly);
            services.AddSingleton(provider => configuration);
            services.AddLogging(builder => builder.AddConsole().AddConfiguration(configuration.GetSection("Logging")))
                .AddTransient<IFileStorageService, FileStorageService>()
                .AddTransient<ISeedService, SeedService>()
                .BuildServiceProvider();
        }
    }
}