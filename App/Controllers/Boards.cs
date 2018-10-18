using System.Text;
using System;
using Microsoft.AspNetCore.Http;

namespace Kandu.Pages
{
    public class Boards : Page
    {
        public Boards(HttpContext context) : base(context)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            if(User.userId == 0)
            {
                //load login page
                var page = new Login(context);
                return page.Render(path);
            }
            //load boards list
            var scaffold = new Scaffold("/Views/Boards/boards.html", Server.Scaffold);
            
            var boards = Query.Boards.GetList(User.userId);
            var html = new StringBuilder();
            var item = new Scaffold("/Views/Boards/list-item.html", Server.Scaffold);
            boards.ForEach((Query.Models.Board b) => {
                item.Data["favorite"] = b.favorite ? "1" : "";
                item.Data["name"] = b.name;
                item.Data["color"] = "#" + b.color;
                item.Data["extra"] = b.favorite ? "fav" : "";
                item.Data["id"] = b.boardId.ToString();
                item.Data["type"] = b.type.ToString();
                item.Data["url"] = Uri.EscapeUriString("/board/" + b.boardId + "/" + b.name.Replace(" ", "-").ToLower());
                html.Append(item.Render());
            });
            scaffold.Data["list"] = html.ToString();

            //load teams list
            var teams = Query.Teams.GetList(User.userId);
            html = new StringBuilder();
            teams.ForEach((Query.Models.Team t) =>
            {
                html.Append("<option value=\"" + t.teamId + "\">" + t.name + "</option>\n");
            });
            scaffold.Data["team-options"] = html.ToString();

            //load page resources
            AddScript("/js/dashboard.js");
            AddCSS("/css/dashboard.css");

            //load header
            LoadHeader(ref scaffold, false);

            //render page
            return base.Render(path, scaffold.Render());
        }
    }
}
