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
                    page.Init(Context, Parameters, Path, PathParts);
                    break;
                    
            }
            if(page == null) { return Error404(); }
            LoadPartial(ref page);
            return page.Render(body);
        }
    }
}
