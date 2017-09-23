using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace Kandu
{
    public class Startup
    {

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            //set up server-side memory cache
            services.AddDistributedMemoryCache();
            services.AddMemoryCache();

            services.AddSession(opts =>
            {
                //set up cookie expiration
                opts.Cookie.Name = "Websilk";
                opts.IdleTimeout = TimeSpan.FromMinutes(60);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //load application-wide memory store
            Server server = new Server();

            //use session
            app.UseSession();

            //handle static files
            var options = new StaticFileOptions { ContentTypeProvider = new FileExtensionContentTypeProvider() };
            app.UseStaticFiles(options);

            //exception handling
            var errOptions = new DeveloperExceptionPageOptions();
            errOptions.SourceCodeLineCount = 10;
            app.UseDeveloperExceptionPage();

            var config = new ConfigurationBuilder()
                .AddJsonFile(server.MapPath("config.json"))
                .AddEnvironmentVariables().Build();

            server.sqlActive = config.GetSection("Data:Active").Value;
            server.sqlConnection = config.GetSection("Data:" + server.sqlActive).Value;

            var isdev = false;
            switch (config.GetSection("Environment").Value.ToLower())
            {
                case "development":
                case "dev":
                    server.environment = Server.enumEnvironment.development;
                    isdev = true;
                    break;
                case "staging":
                case "stage":
                    server.environment = Server.enumEnvironment.staging;
                    break;
                case "production":
                case "prod":
                    server.environment = Server.enumEnvironment.production;
                    break;
            }

            //configure server security
            server.bcrypt_workfactor = int.Parse(config.GetSection("Encryption:bcrypt_work_factor").Value);
            
            //Debugging capabilities
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            //raise event signaling the server is up and running
            server.Up();

            //Set Up MVC Routing
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
