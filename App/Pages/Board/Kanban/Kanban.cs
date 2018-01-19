using System;
using System.Text;

namespace Kandu.Pages
{
    public class Kanban : BoardPage
    {
        public Kanban(Core KanduCore) : base(KanduCore)
        {
            //load page resources
            scripts += "<script src=\"/js/pages/board/kanban/kanban.js\"></script>";
            headCss += "<link type=\"text/css\" rel=\"stylesheet\" href=\"/css/pages/board/kanban/kanban.css\">";
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            var html = new StringBuilder();

            if(path.Length > 1)
            {
                //load kanban lists for board
                var boardId = int.Parse(path[1]);
                var scaffold = new Scaffold("Pages/Board/Kanban/kanban.html", S.Server.Scaffold);
                var kanban = new Services.List.Kanban(S);
                var htmlists = new StringBuilder();
                foreach(var list in board.lists)
                {
                    htmlists.Append(kanban.LoadListHtml(list, list.cards) + "\n");
                }

                var colors = new Utility.Colors();

                scaffold.Data["name"] = board.name;
                try
                {
                    scaffold.Data["color-hover"] = colors.FromHexToRgba("#" + board.color, 1);
                }
                catch (Exception ex)
                {
                }
                scaffold.Data["lists"] = htmlists.ToString();
                html.Append(scaffold.Render());
            }

            return html.ToString();
        }
    }
}
