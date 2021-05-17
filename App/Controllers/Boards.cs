using System.Text;
using System;

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
            
            var boards = Query.Boards.GetList(User.userId);
            var html = new StringBuilder();
            var item = new View("/Views/Boards/list-item.html");
            boards.ForEach((Query.Models.Board b) => {
                item["favorite"] = b.favorite ? "1" : "";
                item["name"] = b.name;
                item["color"] = "#" + b.color;
                item["extra"] = b.favorite ? "fav" : "";
                item["id"] = b.boardId.ToString();
                item["type"] = b.type.ToString();
                item["url"] = Uri.EscapeUriString("/board/" + b.boardId + "/" + b.name.Replace(" ", "-").ToLower());
                html.Append(item.Render());
            });
            view["list"] = html.ToString();

            //load page resources
            AddScript("/js/dashboard.js?v=" + Server.Version);
            AddCSS("/css/dashboard.css?v=" + Server.Version);

            //load header
            LoadHeader(ref view, HasMenu.Boards);

            //render page
            return base.Render(view.Render());
        }
    }
}
