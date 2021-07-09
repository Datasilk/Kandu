using System;
using System.Text;

namespace Kandu.Common.Card
{
    public static class Kanban
    {
        public static string RenderCard(Query.Models.Card card)
        {
            var useLayout = false;
            View cardview;
            if(card.name.IndexOf("----") == 0)
            {
                //separator
                cardview = new View("/Views/Card/Kanban/Type/separator.html");
            }
            else if(card.name.IndexOf("# ") == 0)
            {
                //header
                cardview = new View("/Views/Card/Kanban/Type/header.html");
                cardview["name"] = card.name.TrimStart(new char[] { '#', ' ' });
            }
            else
            {
                //card
                cardview = new View("/Views/Card/Kanban/Type/card.html");
                useLayout = true;
            }
            
            if(useLayout == true)
            {
                //load card custom design
                var view = new View("/Views/Card/Kanban/Layout/" + card.layout.ToString() + ".html");

                if(card.name.IndexOf("[x]") == 0 || card.name.IndexOf("[X]") == 0)
                {
                    var checkmark = new View("/Views/Card/Kanban/Elements/checkmark.html");
                    view["name"] = checkmark.Render() + card.name.Substring(4);
                }
                else if (card.name.IndexOf("[!]") == 0 || card.name.IndexOf("[!]") == 0)
                {
                    var checkmark = new View("/Views/Card/Kanban/Elements/warning.html");
                    view["name"] = checkmark.Render() + card.name.Substring(4);
                }
                else
                {
                    view["name"] = card.name;
                }
                
                view["colors"] = "";

                //render custom design inside card container
                cardview["layout"] = view.Render();
            }

            //load card container
            cardview["id"] = card.cardId.ToString();

            //render card container
            return cardview.Render();
        }

        public static Tuple<Query.Models.Card, string> Details(int boardId, int cardId)
        {
            try
            {
                var card = Query.Cards.GetDetails(boardId, cardId);
                var view = new View("/Views/Card/Kanban/details.html");
                view["title"] = card.name;
                view["list-name"] = card.listName;
                view["description"] = card.description;
                view["no-description"] = card.description.Length > 0 ? "hide" : "";
                view["has-description"] = card.description.Length <= 0 ? "hide" : "";
                view["archive-class"] = card.archived ? "hide" : "";
                view["restore-class"] = card.archived ? "" : "hide";
                view["delete-class"] = card.archived ? "" : "hide";
                return new Tuple<Query.Models.Card, string>(card, view.Render());
            }
            catch (Exception)
            {
                throw new ServiceErrorException("Error loading card details");
            }
        }

        public static string RenderCardsForMember(int userId, int orgId = 0)
        {
            var html = new StringBuilder();
            var cards = Query.Cards.AssignedToMember(userId, orgId);
            foreach (var card in cards)
            {
                html.Append(RenderCard(card) + "\n");
            }
            return html.ToString();
        }
    }
}
