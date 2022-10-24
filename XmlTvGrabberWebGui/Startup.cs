using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XmlTvGrabberWebGui.Data;
using XmlTvGrabberWebGui.Helpers;
using XmlTvGrabberWebGui.Helpers.GlobalProperties;
using XmlTvGrabberWebGui.Helpers.Logger;

namespace XmlTvGrabberWebGui
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddForEvolveLocalization(options =>
            {
                options.MvcOptions.EnableViewLocalization = true;
                options.RequestLocalizationOptions.DefaultRequestCulture = new RequestCulture("fr", "fr");
                options.RequestLocalizationOptions.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider()
                };
            });

            services.AddServerSideBlazor();
            services.AddControllersWithViews();

            services.AddDbContext<GrabberContext>(o => o.UseSqlite("Data Source=grabber.db").UseLazyLoadingProxies());

            services
                .AddResponseCompression(o => 
                {
                    o.Providers.Add<BrotliCompressionProvider>();
                    o.Providers.Add<GzipCompressionProvider>();
                })
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    // Source: https://stackoverflow.com/questions/55666826/where-did-imvcbuilder-addjsonoptions-go-in-net-core-3-0
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = null;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                })
                .AddForEvolveMvcLocalization();

            services
                .Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest)
                .Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest)
                .AddSingleton<IGlobalProperties, GlobalProperties>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

            // Mise à jour automatique de la base de données
            UpdateDatabase(serviceScopeFactory);

            // Initialisation du logger
            loggerFactory.AddProvider(new SqliteLoggerProvider(serviceScopeFactory));

            app.UseForEvolveRequestLocalization();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseResponseCompression();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapBlazorHub();
            });
        }

        private static void UpdateDatabase(IServiceScopeFactory serviceScopeFactory)
        {
            using var serviceScope = serviceScopeFactory.CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<GrabberContext>();
            context.Database.Migrate();
        }

    }
}
