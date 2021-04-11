namespace Kandu.Controllers
{
    public class Import : Controller
    {
        public override string Render(string body = "")
        {
            Controller page = null;
            switch (PathParts[1].ToLower())
            {
                case "trello":
                    page = new Imports.Trello();
                    page.Context = Context;
                    page.Parameters = Parameters;
                    page.Path = Path;
                    page.PathParts = PathParts;
                    page.Init();
                    break;
                    
            }
            if(page == null) { return Error404(); }
            LoadPartial(ref page);
            return page.Render(body);
        }
    }
}
