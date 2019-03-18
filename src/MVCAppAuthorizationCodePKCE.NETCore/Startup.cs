using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace MVCAppAuthorizationCodePKCE.NETCore
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
            services.AddMemoryCache();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Register the settings for our application as a Singleton
            services.Configure<AppConfiguration>(Configuration.GetSection("AppConfiguration"));
            services.AddSingleton(resolver =>
                resolver.GetRequiredService<IOptions<AppConfiguration>>().Value);

            var appConfig = Configuration.GetSection(nameof(AppConfiguration)).Get<AppConfiguration>();

            // Add our typed Tapkey API HttpClient
            services.AddHttpClient<ITapkeyApiClient, TapkeyApiClient>(client =>
            {
                // Set the base address of the Tapkey API from the configuration
                client.BaseAddress = new Uri(appConfig.TapkeyApiBaseUrl);

                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "MVCAppAuthorizationCodePKCE.NETCore");
            });

            // Add a HttpClient for the Tapkey Authorization Server
            services.AddHttpClient(AppConstants.TapkeyAuthorizationServerClient, client =>
            {
                client.BaseAddress = new Uri(appConfig.TapkeyAuthorizationServerUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "MVCAppAuthorizationCodePKCE.NETCore");
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
