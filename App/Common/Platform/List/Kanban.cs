using System.Collections.Generic;
using System.Text;

namespace Kandu.Common.Platform.List
{
    public static class Kanban
    {
        public static string RenderList(Query.Models.List list, List<Query.Models.Card> cards)
        {
            Server Server = Server.Instance;

            //load html templates
            var scaffold = new Scaffold("/Views/List/Kanban/list.html", Server.Scaffold);
            var html = new StringBuilder();

            //set up each card
            foreach (var card in cards)
            {
                html.Append(Card.Kanban.RenderCard(card));
            }

            //set up list
            scaffold.Data["id"] = list.listId.ToString();
            scaffold.Data["title"] = list.name;
            scaffold.Data["items"] = html.ToString();

            //render list
            return scaffold.Render();
        }
    }
}
