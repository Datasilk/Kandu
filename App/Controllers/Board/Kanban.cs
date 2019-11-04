using System;
using System.Text;

namespace Kandu.Controllers
{
    public class Kanban : BoardPage
    {
        public Kanban()
        {
            //load page resources
            Scripts.Append("<script src=\"/js/views/board/kanban/kanban.js?v=" + Server.Version + "\"></script>");
            Css.Append("<link type=\"text/css\" rel=\"stylesheet\" href=\"/css/views/board/kanban/kanban.css?v=" + Server.Version + "\">");
        }

        public override string Render(string body = "")
        {
            var html = new StringBuilder();

            if(PathParts.Length > 1)
            {
                //load kanban lists for board
                var view = new View("Views/Board/Kanban/kanban.html");
                var htmlists = new StringBuilder();
                foreach(var list in board.lists)
                {
                    htmlists.Append(Common.Platform.List.Kanban.RenderList(list, list.cards) + "\n");
                }

                var colors = new Utility.Colors();

                view["name"] = board.name;
                try
                {
                    view["color-hover"] = colors.FromHexToRgba("#" + board.color, 1);
                }
                catch (Exception)
                {
                }
                view["lists"] = htmlists.ToString();
                html.Append(view.Render());
            }
            return html.ToString();
        }
    }
}
