using Microsoft.AspNetCore.Http;

namespace Kandu.Pages
{
    public class Import : Page
    {
        public Import(HttpContext context) : base(context)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            Page page = null;
            switch (path[1].ToLower())
            {
                case "trello":
                    page = new Imports.Trello(context);
                    break;
                    
            }
            if(page == null) { return Error404(); }
            LoadPartial(ref page);
            return page.Render(path, body, metadata);
        }
    }
}
