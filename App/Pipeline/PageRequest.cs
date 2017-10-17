using System;
using Microsoft.AspNetCore.Http;

namespace Kandu.Pipeline
{
    public class PageRequest
    {
        private Core S;
        public Scaffold scaffold;

        public PageRequest(Server server, HttpContext context)
        {
            //the Pipeline.PageRequest is simply the first page request for a Kandu website. 

            S = new Core(server, context);

            var path = context.Request.Path.ToString().Substring(1).Split('?', 2)[0].Split('/');

            var page = GetWebPage(("Kandu.Pages." + (path[0] == "" ? "Home" : S.Util.Str.Capitalize(path[0].Replace("-"," ")).Replace(" ",""))));

            //render the server response
            S.Response.ContentType = "text/html";
            S.Response.WriteAsync(page.Render(path));
        }

        private Page GetWebPage(string className)
        {
            //hard-code all known services to increase server performance
            switch (className)
            {
                case "Kandu.Pages.Login":
                    return new Pages.Login(S);

                default:
                    //last resort, find service class manually
                    Type type = Type.GetType(className);
                    return (Page)Activator.CreateInstance(type, new object[] { S });
            }
        }
    }
}
