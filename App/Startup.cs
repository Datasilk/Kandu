using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Datasilk.Core.Extensions;

namespace Kandu
{
    public class Startup
    {
        protected static IConfigurationRoot config;

        public virtual void ConfigureServices(IServiceCollection services)
        {
            //set up Server-side memory cache
            services.AddDistributedMemoryCache();
            services.AddMemoryCache();

            //configure request form options
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });

            //add session
            services.AddSession();

            //add hsts
            services.AddHsts(options => { });
            services.AddHttpsRedirection(options => { });
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //set root Server path
            var path = env.ContentRootPath + "\\";

            Server.RootPath = path;

            //get environment based on application build
            switch (env.EnvironmentName.ToLower())
            {
                case "production":
                    Server.environment = Server.Environment.production;
                    break;
                case "staging":
                    Server.environment = Server.Environment.staging;
                    break;
                default:
                    Server.environment = Server.Environment.development;
                    break;
            }

            //load application-wide cache
            var configFile = "config" + (Server.environment == Server.Environment.production ? ".prod" : "") + ".json";
            config = new ConfigurationBuilder()
                .AddJsonFile(Server.MapPath(configFile))
                .AddEnvironmentVariables().Build();

            Server.config = config;

            //configure Server defaults
            Server.nameSpace = config.GetSection("assembly").Value;
            Server.hostUrl = config.GetSection("hostUrl").Value;
            var servicepaths = config.GetSection("servicePaths").Value;
            if (servicepaths != null && servicepaths != "")
            {
                Server.servicePaths = servicepaths.Replace(" ", "").Split(',');
            }
            if (config.GetSection("version").Value != null)
            {
                Server.Version = config.GetSection("version").Value;
            }

            //configure Server database connection strings
            Server.sqlActive = config.GetSection("sql:Active").Value;
            Server.sqlConnectionString = config.GetSection("sql:" + Server.sqlActive).Value;

            //configure Server security
            Server.bcrypt_workfactor = int.Parse(config.GetSection("Encryption:bcrypt_work_factor").Value);
            Server.salt = config.GetSection("Encryption:salt").Value;

            //configure cookie-based authentication
            var expires = !string.IsNullOrEmpty(config.GetSection("Session:Expires").Value) ? int.Parse(config.GetSection("Session:Expires").Value) : 60;

            //use session
            var sessionOpts = new SessionOptions();
            sessionOpts.Cookie.Name = Server.nameSpace;
            sessionOpts.IdleTimeout = TimeSpan.FromMinutes(expires);
            app.UseSession(sessionOpts);

            //handle static files
            var provider = new FileExtensionContentTypeProvider();

            // Add static file mappings
            provider.Mappings[".svg"] = "image/svg";
            var options = new StaticFileOptions
            {
                ContentTypeProvider = provider
            };
            app.UseStaticFiles(options);

            //exception handling
            if (Server.environment == Server.Environment.development)
            {
                //app.UseDeveloperExceptionPage(new DeveloperExceptionPageOptions
                //{
                //    SourceCodeLineCount = 10
                //});
            }
            else
            {
                //use HTTPS
                app.UseHsts();
                app.UseHttpsRedirection();

                //use health checks
                app.UseHealthChecks("/health");
            }

            //use HTTPS
            //app.UseHttpsRedirection();

            //set up database connection
            Query.Sql.connectionString = Server.sqlConnectionString;
            Server.resetPass = Query.Users.HasPasswords();
            Server.hasAdmin = Query.Users.HasAdmin();

            //run Datasilk application
            app.UseDatasilkMvc(new MvcOptions()
            {
                Routes = new Routes(),
                IgnoreRequestBodySize = true,
                WriteDebugInfoToConsole = false,
                LogRequests = false,
                InvokeNext = false
            });
        }
    }
}
