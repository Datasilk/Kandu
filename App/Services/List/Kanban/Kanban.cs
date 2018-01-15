using System.Collections.Generic;
using System.Text;

namespace Kandu.Services.List
{
    public class Kanban: Service
    {
        public Kanban(Core DatasilkCore) : base(DatasilkCore)
        {
        }

        public string LoadList(int listId)
        {
            var query = new Query.Lists(S.Server.sqlConnectionString);
            var cards = new Query.Cards(S.Server.sqlConnectionString);
            var list = query.GetDetails(listId);
            return LoadListHtml(list, cards.GetList(list.boardId, listId, 1, 100));
        }

        public string LoadListHtml(Query.Models.List list, List<Query.Models.Card> cards)
        {
            //load html templates
            var scaffold = new Scaffold("/Services/List/Kanban/list.html", S.Server.Scaffold);
            var serviceCards = new Card.Kanban(S);
            var html = new StringBuilder();

            //set up each card
            foreach(var card in cards)
            {
                html.Append(serviceCards.LoadCardHtml(card));
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
