namespace Kandu.Controllers
{
    public class Boards : Controller
    {
        public override string Render(string body = "")
        {
            if(User.userId == 0)
            {
                //load login page
                var page = new Login();
                page.Context = Context;
                page.Parameters = Parameters;
                page.Path = Path;
                page.PathParts = PathParts;
                page.Init();
                return page.Render();
            }
            //load boards list
            var view = new View("/Views/Boards/boards.html");

            view["list"] = Common.Platform.Boards.RenderList(this);

            //load page resources
            AddScript("/js/dashboard.js?v=" + Server.Version);
            AddCSS("/css/dashboard.css?v=" + Server.Version);

            //load header
            LoadHeader(ref view, HasMenu.Boards);

            //init boards page
            Scripts.Append("<script>S.boards.page.init();</script>");

            //render page
            return base.Render(view.Render());
        }
    }
}
