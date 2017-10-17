using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Kandu
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //set up server-side memory cache
            services.AddDistributedMemoryCache();
            services.AddMemoryCache();

            services.AddSession(opts =>
            {
                //set up cookie expiration
                opts.Cookie.Name = "Kandu";
                opts.IdleTimeout = TimeSpan.FromMinutes(60);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            //load application-wide memory store
            Server server = new Server();

            //use session
            app.UseSession();

            //handle static files
            var options = new StaticFileOptions {ContentTypeProvider = new FileExtensionContentTypeProvider()};
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
                case "development": case "dev":
                    server.environment = Server.enumEnvironment.development;
                    isdev = true;
                    break;
                case "staging": case "stage":
                    server.environment = Server.enumEnvironment.staging;
                    break;
                case "production": case "prod":
                    server.environment = Server.enumEnvironment.production;
                    break;
            }

            //configure server security
            server.bcrypt_workfactor = int.Parse(config.GetSection("Encryption:bcrypt_work_factor").Value);

            server.Up();

            //run Kandu application
            app.Run(async (context) =>
            {
                var requestStart = DateTime.Now;
                DateTime requestEnd;
                TimeSpan tspan;
                var requestType = "";
                var path = cleanPath(context.Request.Path.ToString());
                var paths = path.Split('/').Skip(1).ToArray();                
                var extension = "";

                //get request file extension (if exists)
                if(path.IndexOf(".")>= 0)
                {
                    for (int x = path.Length - 1; x >= 0; x += -1)
                    {
                        if (path.Substring(x, 1) == ".")
                        {
                            extension = path.Substring(x + 1); return;
                        }
                    }
                }
                
                server.requestCount += 1;
                if (isdev)
                {
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine("{0} GET {1}", DateTime.Now.ToString("hh:mm:ss"), context.Request.Path);
                }

                if (paths.Length > 1)
                {
                    if(paths[0]=="api")
                    {
                        //run a web service via ajax (e.g. /api/namespace/class/function)
                         IFormCollection form = null;
                        if(context.Request.ContentType != null)
                        {
                            if (context.Request.ContentType.IndexOf("application/x-www-form-urlencoded") >= 0)
                            {
                            }else if (context.Request.ContentType.IndexOf("multipart/form-data") >= 0)
                            {
                                //get files collection from form data
                                form = await context.Request.ReadFormAsync();
                            }
                        }
                        
                        if (cleanNamespace(paths))
                        {
                            //execute web service
                            var ws = new Pipeline.WebService(server, context, paths, form);
                            requestType = "service";
                        }
                    }
                }

                if(requestType == "" && extension == "")
                {
                    //initial page request
                    var r = new Pipeline.PageRequest(server, context);
                    requestType = "page";
                }

                if (isdev)
                {
                    requestEnd = DateTime.Now;
                    tspan = requestEnd - requestStart;
                    server.requestTime += (tspan.Seconds);
                    Console.WriteLine("END GET {0} {1} ms {2}", context.Request.Path, tspan.Milliseconds, requestType);
                    Console.WriteLine("");
                }
            });
        }

        private string cleanPath(string path)
        {
            //check for malicious path input
            if(path == "") { return path; }

            if (path.Replace("/", "").Replace("-","").Replace("+","").All(char.IsLetterOrDigit)) {
                //path is clean
                return path;
            }

            //path needs to be cleaned
            return path
                .Replace("{", "")
                .Replace("}", "")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace(":","")
                .Replace("$","")
                .Replace("!","")
                .Replace("*","");
        }

        private bool cleanNamespace(string[] paths)
        {
            //check for malicious namespace in web service request
            foreach(var p in paths)
            {
                if (!p.All(a => char.IsLetter(a))) { return false; }
            }
            return true;
        }
    }
}
