using System.Text;

namespace Kandu.Pages
{
    public class Kanban : Page
    {
        public Kanban(Core KanduCore) : base(KanduCore)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            var html = new StringBuilder();

            if(path.Length > 1)
            {
                //load kanban lists for board
                var boardId = int.Parse(path[1]);
                var boards = new Query.Boards(S.Server.sqlConnection);
                var board = boards.GetBoardDetails(boardId);
                var scaffold = new Scaffold(S, "Lists/Kanban/kanban.html");
                scaffold.Data["color"] = "#" + board.color;
                scaffold.Data["name"] = board.name;
                html.Append(scaffold.Render());

                //load page resources
                scripts += "<script src=\"/js/lists/kanban/kanban.js\"></script>";
                headCss += "<link type=\"text/css\" rel=\"stylesheet\" href=\"/css/lists/kanban/kanban.css\">";
            }

            return base.Render(path, html.ToString());
        }
    }
}
