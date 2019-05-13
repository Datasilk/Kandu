using Microsoft.AspNetCore.Http;

namespace Kandu.Controllers
{
    public class Import : Controller
    {
        public Import(HttpContext context, Parameters parameters) : base(context, parameters)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            Controller page = null;
            switch (path[1].ToLower())
            {
                case "trello":
                    page = new Imports.Trello(context, parameters);
                    break;
                    
            }
            if(page == null) { return Error404(); }
            LoadPartial(ref page);
            return page.Render(path, body, metadata);
        }
    }
}
