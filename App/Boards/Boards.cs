using System.Text;

namespace Kandu.Pages
{
    public class Boards : Page
    {
        public Boards(Core KanduCore) : base(KanduCore)
        {
        }

        public override string Render(string[] path, string body = "")
        {
            if(S.User.userId == 0)
            {
                //load login page
                var page = new Login(S);
                return page.Render(path);
            }
            //load boards list
            var scaffold = new Scaffold(S, "/Boards/boards.html");
            scripts += "<script src=\"/js/boards/boards.js\"></script>";

            var query = new Query.Boards(S.Server.sqlConnection);
            var boards = query.GetList(S.User.userId);
            var html = new StringBuilder();
            var item = new Scaffold(S, "/Boards/list-item.html");
            boards.ForEach((Query.Models.Board b) => {
                item.Data["favorite"] = b.favorite ? "1" : "";
                item.Data["name"] = b.name;
                item.Data["color"] = "#" + b.color;
                item.Data["extra"] = b.favorite ? "fav" : "";
                html.Append(item.Render());
            });
            scaffold.Data["list"] = html.ToString();

            //load teams list
            var queryTeams = new Query.Teams(S.Server.sqlConnection);
            var teams = queryTeams.GetList(S.User.userId);
            html = new StringBuilder();
            teams.ForEach((Query.Models.Team t) =>
            {
                html.Append("<option value=\"" + t.teamId + "\">" + t.name + "</option>\n");
            });
            scaffold.Data["team-options"] = html.ToString();

            headCss += "<link type=\"text/css\" rel=\"stylesheet\" href=\"/css/board.css\">";
            
            return base.Render(path, scaffold.Render());
        }
    }
}
