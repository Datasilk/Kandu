using System.Text;

namespace Kandu.Pages
{
    public class Boards : Page
    {
        public Boards(Core KanduCore) : base(KanduCore)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            if(S.User.userId == 0)
            {
                //load login page
                var page = new Login(S);
                return page.Render(path);
            }
            //load boards list
            var scaffold = new Scaffold("/Pages/Boards/boards.html", S.Server.Scaffold);

            var query = new Query.Boards(S.Server.sqlConnectionString);
            var boards = query.GetList(S.User.userId);
            var html = new StringBuilder();
            var item = new Scaffold("/Pages/Boards/list-item.html", S.Server.Scaffold);
            boards.ForEach((Query.Models.Board b) => {
                item.Data["favorite"] = b.favorite ? "1" : "";
                item.Data["name"] = b.name;
                item.Data["color"] = "#" + b.color;
                item.Data["extra"] = b.favorite ? "fav" : "";
                item.Data["id"] = b.boardId.ToString();
                item.Data["type"] = b.type.ToString();
                item.Data["url"] = S.Util.Str.UrlEncode("/board/" + b.boardId + "/" + b.name.Replace(" ", "-").ToLower());
                html.Append(item.Render());
            });
            scaffold.Data["list"] = html.ToString();

            //load teams list
            var queryTeams = new Query.Teams(S.Server.sqlConnectionString);
            var teams = queryTeams.GetList(S.User.userId);
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
