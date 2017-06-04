using ComicBoxApi.App;
using ComicBoxApi.App.Cache;
using ComicBoxApi.App.FileBrowser;
using ComicBoxApi.App.Imaging;
using ComicBoxApi.App.Worker;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;

namespace ComicBoxApi
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build()
                .Run();
        }

        public static IWebHostBuilder CreateDefaultBuilder(string[] args)
        {
            var builder = new WebHostBuilder()
                .ConfigureAppConfiguration(BuildAppConfiguration(args))
                .UseConfiguration(BuildConfiguration(args))
                .UseKestrel()
                .UseIISIntegration()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureLogging(BuildLogging());

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

        private static IConfiguration BuildConfiguration(string[] args)
        {
            // Hack, check how dotnet team handle this in dotnet 2.0 preview
            return new ConfigurationBuilder().AddCommandLine(args).Build();
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

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // Add framework services.                
            services.AddMemoryCache();

            var externalFileProvider = new PhysicalFileProvider(_configuration["Settings:AbsoluteBasePath"]);
            var compositeProvider = new CompositeFileProvider(externalFileProvider);
            services.AddSingleton<IFileProvider>(compositeProvider);

            services.AddSingleton<ICacheService, CacheService>();            
            services.AddSingleton<IImageService, ImageService>();

            services.AddTransient<IBookInfoService, BookInfoService>();
            services.AddTransient<IFilePathFinder, FilePathFinder>();
            services.AddSingleton<ThumbnailWorker, ThumbnailWorker>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.ApplicationServices.GetService<ThumbnailWorker>().Start();
            app.Use(async (context, next) => 
            {
                var thumbnailWorker = context.RequestServices.GetService<ThumbnailWorker>();
                if (!thumbnailWorker.GetStatus().IsInProgress)
                {
                    await thumbnailWorker.Stop();
                }

                await next();
            });

            // UseDeveloperExceptionPage before UseMvc
            app.UseDeveloperExceptionPage();

            app.Use(async (context, next) =>
            {
                await next();

                // If there's no available route, rewrite request to use app root
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/";
                    context.Response.StatusCode = 200;
                    await next();
                }
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();
            
            app.UseMvc();
        }
    }
}
