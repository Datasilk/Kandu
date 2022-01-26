using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Datasilk.Core.Extensions;
using Kandu.Common;

namespace Kandu
{
    public class Startup
    {
        private static IConfigurationRoot config;
        private List<Assembly> assemblies = new List<Assembly> { Assembly.GetCallingAssembly() };

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

            //add health checks
            services.AddHealthChecks();

            //add hsts
            services.AddHsts(options => { });
            services.AddHttpsRedirection(options => { });

            //try deleting Vendors that are marked for uninstallation
            Vendors.DeleteVendors();

            //get list of assemblies for Vendor related functionality
            if (!assemblies.Contains(Assembly.GetExecutingAssembly()))
            {
                assemblies.Add(Assembly.GetExecutingAssembly());
            }
            if (!assemblies.Contains(Assembly.GetEntryAssembly()))
            {
                assemblies.Add(Assembly.GetEntryAssembly());
            }

            //get a list of DLLs in the Vendors folder (if any)
            var vendorDLLs = Vendors.LoadDLLs();

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //get list of vendor classes that inherit IVendorInfo interface
            foreach (var assembly in assemblies)
            {
                //get a list of interfaces from the assembly
                var types = assembly.GetTypes()
                    .Where(type => typeof(Vendor.IVendorInfo).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract).ToList();
                foreach (var type in types)
                {
                    Vendors.GetInfoFromType(type);
                }
            }
            //get list of DLLs that contain the IVendorInfo interface
            Vendors.GetInfoFromFileSystem();
            var vendorCount = Core.Vendors.Details.Where(a => a.Version != "").Count();
            Console.WriteLine("Found " + vendorCount + " Vendor" + (vendorCount != 1 ? "s" : ""));

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //get list of vendor classes that inherit IVendorStartup interface
            foreach (var assembly in assemblies)
            {
                //get a list of interfaces from the assembly
                var types = assembly.GetTypes()
                    .Where(type => typeof(Vendor.IVendorStartup).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract).ToList();
                foreach (var type in types)
                {
                    Vendors.GetStartupFromType(type);
                }
            }
            //get list of DLLs that contain the IVendorStartup interface
            Vendors.GetStartupsFromFileSystem();
            Console.WriteLine("Found " + Core.Vendors.Startups.Count + " Vendor Startup Class" + (Core.Vendors.Startups.Count != 1 ? "es" : ""));

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //get list of vendor classes that inherit IVendorController interface
            foreach (var assembly in assemblies)
            {
                //get a list of interfaces from the assembly
                var types = assembly.GetTypes()
                    .Where(type => typeof(Vendor.IVendorController).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract).ToList();
                foreach (var type in types)
                {
                    Vendors.GetControllerFromType(type);
                }
            }
            //get list of DLLs that contain the IVendorController interface
            Vendors.GetControllersFromFileSystem();
            Console.WriteLine("Found " + Core.Vendors.Controllers.Count + " Vendor Controller" + (Core.Vendors.Controllers.Count != 1 ? "s" : ""));

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //get list of vendor classes that inherit IVendorPartialView interface
            foreach (var assembly in assemblies)
            {
                //get a list of interfaces from the assembly
                var types = assembly.GetTypes()
                    .Where(type => typeof(Vendor.IVendorPartialView).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract).ToList();
                foreach (var type in types)
                {
                    Vendors.GetPartialViewsFromType(type);
                }
            }
            //get list of DLLs that contain the IVendorController interface
            Vendors.GetPartialViewsFromFileSystem();
            Console.WriteLine("Found " + Core.Vendors.Controllers.Count + " Vendor Partial View" + (Core.Vendors.PartialViewsUnsorted.Count != 1 ? "s" : ""));

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //get list of vendor classes that inherit IVendorKeys interface
            foreach (var assembly in assemblies)
            {
                //get a list of interfaces from the assembly
                var types = assembly.GetTypes()
                    .Where(type => typeof(Vendor.IVendorKeys).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract).ToList();
                foreach (var type in types)
                {
                    Vendors.GetSecurityKeysFromType(type);
                }
            }
            //get list of DLLs that contain the IVendorKeys interface
            Vendors.GetSecurityKeysFromFileSystem();
            var totalKeys = 0;
            foreach (var chain in Core.Vendors.Keys)
            {
                totalKeys += chain.Keys.Length;
            }
            Console.WriteLine("Found " + Core.Vendors.Keys.Count + " Vendor" + (Core.Vendors.Keys.Count != 1 ? "s" : "") + " with Security Keys (" + totalKeys + " key" + (totalKeys != 1 ? "s" : "") + ")");

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //get list of vendor classes that inherit IVendorEmailClient interface
            foreach (var assembly in assemblies)
            {
                //get a list of interfaces from the assembly
                var types = assembly.GetTypes()
                    .Where(type => typeof(Vendor.IVendorEmailClient).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract).ToList();
                foreach (var type in types)
                {
                    Vendors.GetEmailClientsFromType(type);
                }
            }
            //get list of DLLs that contain the IVendorEmailClient interface
            Vendors.GetEmailClientsFromFileSystem();
            Console.WriteLine("Found " + Core.Vendors.EmailClients.Count + " Vendor Email Client" + (Core.Vendors.EmailClients.Count != 1 ? "s" : ""));

            
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //get list of vendor classes that inherit KanduEvents abstract class
            foreach (var assembly in assemblies)
            {
                //get a list of abstract classes from the assembly
                var types = assembly.GetTypes()
                    .Where(type => typeof(Vendor.KanduEvents).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract).ToList();
                foreach (var type in types)
                {
                    Vendors.GetKanduEventsFromType(type);
                }
            }
            //get list of DLLs that contain the IVendorKeys interface
            Vendors.GetKanduEventsFromFileSystem();
            Console.WriteLine("Found " + Core.Vendors.EventHandlers.Count + " Vendor" + (Core.Vendors.EventHandlers.Count != 1 ? "s" : "") + " That listen to Kandu Events");

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //execute ConfigureServices method for all vendors that use IVendorStartup interface
            foreach (var kv in Core.Vendors.Startups)
            {
                var vendor = (Vendor.IVendorStartup)Activator.CreateInstance(kv.Value);
                try
                {
                    vendor.ConfigureServices(services);
                    Console.WriteLine("Configured Services for " + kv.Key);
                }
                catch (Exception) { }
            }
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //get environment based on application build
            App.Environment = (env.EnvironmentName.ToLower()) switch
            {
                "production" => Environment.production,
                "staging" => Environment.staging,
                _ => Environment.development,
            };

            //load application-wide cache
            var configFile = "config" + (App.Environment == Environment.production ? ".prod" : "") + ".json";
            config = new ConfigurationBuilder()
                .AddJsonFile(App.MapPath(configFile))
                .AddEnvironmentVariables().Build();

            Server.Config = config;

            //configure Server defaults
            App.Host = config.GetSection("hostUrl").Value;
            var servicepaths = config.GetSection("servicePaths").Value;
            if (servicepaths != null && servicepaths != "")
            {
                App.ServicePaths = servicepaths.Replace(" ", "").Split(',');
            }
            if (config.GetSection("version").Value != null)
            {
                Server.Version = config.GetSection("version").Value;
            }

            //configure Server database connection strings
            Server.SqlActive = config.GetSection("sql:Active").Value;
            Server.SqlConnectionString = config.GetSection("sql:" + Server.SqlActive).Value;

            //configure Server security
            Server.BcryptWorkFactor = int.Parse(config.GetSection("Encryption:bcrypt_work_factor").Value);
            Server.Salt = config.GetSection("Encryption:salt").Value;

            //configure cookie-based authentication
            var expires = !string.IsNullOrEmpty(config.GetSection("Session:Expires").Value) ? int.Parse(config.GetSection("Session:Expires").Value) : 60;

            //use session
            var sessionOpts = new SessionOptions();
            sessionOpts.Cookie.Name = "Kandu";
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
            if (App.Environment == Environment.development)
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
            Query.Sql.ConnectionString = Server.SqlConnectionString;
            Server.ResetPass = Query.Users.HasPasswords();
            Server.HasAdmin = Query.Users.HasAdmin();

            //check vendor versions which may run SQL migration scripts
            Vendors.CheckVersions();

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Run any services required after initializing all vendor plugins but before configuring vendor startup services
            Core.Delegates.Email.Send = Email.Send;
            Core.Delegates.PartialViews.Render = PartialViews.Render;
            Core.Delegates.PartialViews.RenderForm = PartialViews.RenderForm;
            Core.Delegates.PartialViews.Save = PartialViews.Save;
            //Core.Delegates.Log.Error = Query.Logs.LogError;

            //execute Configure method for all vendors that use IVendorStartup interface
            foreach (var kv in Core.Vendors.Startups)
            {
                var vendor = (Vendor.IVendorStartup)Activator.CreateInstance(kv.Value);
                try
                {
                    vendor.Configure(app, env, config);
                    Console.WriteLine("Configured Startup for " + kv.Key);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Vendor startup error: " + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }

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
