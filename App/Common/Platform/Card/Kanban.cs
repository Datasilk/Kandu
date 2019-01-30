using System;

namespace Kandu.Common.Platform.Card
{
    public static class Kanban
    {
        public static string RenderCard(Query.Models.Card card)
        {
            var type = "default";
            Scaffold cardscaff;
            if(card.name.IndexOf("----") == 0)
            {
                //separator
                cardscaff = new Scaffold("/Views/Card/Kanban/separator.html");
            }
            else
            {
                //card
                cardscaff = new Scaffold("/Views/Card/Kanban/card.html");
            }
            

            //load card custom design
            var scaffold = new Scaffold("/Views/Card/Kanban/Card/" + type + ".html");
            scaffold.Data["title"] = card.name;
            scaffold.Data["colors"] = "";

            //load card container
            cardscaff.Data["id"] = card.cardId.ToString();

            //render custom design inside card container
            cardscaff.Data["layout"] = scaffold.Render();

            //render card container
            return cardscaff.Render();
        }

        public static Tuple<Query.Models.Card, string> Details(int boardId, int cardId)
        {
            Server Server = Server.Instance;
            try
            {
                var card = Query.Cards.GetDetails(boardId, cardId);
                var scaffold = new Scaffold("/Views/Card/Kanban/details.html", Server.Scaffold);
                scaffold.Data["list-name"] = card.listName;
                scaffold.Data["description"] = card.description;
                scaffold.Data["no-description"] = card.description.Length > 0 ? "hide" : "";
                scaffold.Data["has-description"] = card.description.Length <= 0 ? "hide" : "";
                scaffold.Data["archive-class"] = card.archived ? "hide" : "";
                scaffold.Data["restore-class"] = card.archived ? "" : "hide";
                scaffold.Data["delete-class"] = card.archived ? "" : "hide";
                return new Tuple<Query.Models.Card, string>(card, scaffold.Render());
            }
            catch (Exception)
            {
                throw new ServiceErrorException("Error loading card details");
            }
        }
    }
}
