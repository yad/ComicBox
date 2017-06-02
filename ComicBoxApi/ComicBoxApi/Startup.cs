using ComicBoxApi.App;
using ComicBoxApi.App.Cache;
using ComicBoxApi.App.FileBrowser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace ComicBoxApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // Add framework services.                
            services.AddMemoryCache();

            var externalFileProvider = new PhysicalFileProvider(Configuration["Settings:AbsoluteBasePath"]);
            var compositeProvider = new CompositeFileProvider(externalFileProvider);
            services.AddSingleton<IFileProvider>(compositeProvider);

            services.AddSingleton<ICacheService, CacheService>();

            services.AddTransient<IBookInfoService, BookInfoService>();
            services.AddTransient<IFilePathFinder, FilePathFinder>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // UseDeveloperExceptionPage before UseMvc
            app.UseDeveloperExceptionPage();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc();
        }

        public static IWebHostBuilder CreateDefaultBuilder(string[] args)
        {
            var builder = new WebHostBuilder()
                .ConfigureAppConfiguration(BuildAppConfiguration(args))
                .UseConfiguration(new ConfigurationBuilder().AddCommandLine(args).Build())
                .UseKestrel()
                .UseIISIntegration()
                .UseContentRoot(Directory.GetCurrentDirectory())                
                .ConfigureLogging(BuildLogging());
                
                //.UseDefaultServiceProvider((context, options) => {})
                //.ConfigureServices(BuildServices());

            return builder;
        }

        private static Action<WebHostBuilderContext, IConfigurationBuilder> BuildAppConfiguration(string[] args)
        {
            return (hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;                
                config.SetBasePath(env.ContentRootPath);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
                config.AddEnvironmentVariables();
                config.AddCommandLine(args);
            };
        }

        private static Action<WebHostBuilderContext, LoggerFactory> BuildLogging()
        {
            return (hostingContext, logging) =>
            {
                logging.UseConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
            };
        }

        private static Action<IServiceCollection> BuildServices()
        {
            return services =>
            {
                //services.AddTransient<IConfigureOptions<KestrelServerOptions>, KestrelServerOptionsSetup>();
            };
        }
    }
}
