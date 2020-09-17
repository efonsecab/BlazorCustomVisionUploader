using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using PTI.Microservices.Library.Configuration;
using Microsoft.Extensions.Logging;
using PTI.Microservices.Library.Services;
using PTI.Microservices.Library.Interceptors;

namespace BlazorCustomVisionUploader.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddHttpClient();
            //This key is temporary for demo purposes only and will stop working soon
            //you will need to request your own by writing an email to services@pticostarica.com
            PTI.Microservices.Library.Configuration.GlobalPackageConfiguration.RapidApiKey = 
                "a3893edcbfmsh2efa1861dcc7a10p159864jsnf17e667d1bf7";
            services.AddTransient<ILogger<CustomHttpClientHandler>, Logger<CustomHttpClientHandler>>();
            services.AddTransient<ILogger<AzureBingSearchService>, Logger<AzureBingSearchService>>();
            services.AddTransient<ILogger<AzureCustomVisionService>, Logger<AzureCustomVisionService>>();
            AzureCustomVisionConfiguration azureCustomVisionConfiguration =
                this.Configuration
                .GetSection("AzureConfiguration:AzureCustomVisionConfiguration")
                .Get<AzureCustomVisionConfiguration>();
            AzureBingSearchConfiguration azureBingSearchConfiguration =
                this.Configuration
                .GetSection("AzureConfiguration:AzureBingSearchConfiguration")
                .Get<AzureBingSearchConfiguration>();
            services.AddSingleton<AzureCustomVisionConfiguration>(azureCustomVisionConfiguration);
            services.AddSingleton<AzureBingSearchConfiguration>(azureBingSearchConfiguration);
            services.AddTransient<CustomHttpClientHandler>();
            services.AddTransient<CustomHttpClient>();
            services.AddTransient<AzureBingSearchService>();
            services.AddTransient<AzureCustomVisionService>();
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
