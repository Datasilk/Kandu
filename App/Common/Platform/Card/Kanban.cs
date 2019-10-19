using System;

namespace Kandu.Common.Platform.Card
{
    public static class Kanban
    {
        public static string RenderCard(Query.Models.Card card)
        {
            var useLayout = false;
            Scaffold cardscaff;
            if(card.name.IndexOf("----") == 0)
            {
                //separator
                cardscaff = new Scaffold("/Views/Card/Kanban/Type/separator.html");
            }
            else if(card.name.IndexOf("# ") == 0)
            {
                //header
                cardscaff = new Scaffold("/Views/Card/Kanban/Type/header.html");
                cardscaff["name"] = card.name.TrimStart(new char[] { '#', ' ' });
            }
            else
            {
                //card
                cardscaff = new Scaffold("/Views/Card/Kanban/Type/card.html");
                useLayout = true;
            }
            
            if(useLayout == true)
            {
                //load card custom design
                var scaffold = new Scaffold("/Views/Card/Kanban/Layout/" + card.layout.ToString() + ".html");

                if(card.name.IndexOf("[x]") == 0 || card.name.IndexOf("[X]") == 0)
                {
                    var checkmark = new Scaffold("/Views/Card/Kanban/Elements/checkmark.html");
                    scaffold["name"] = checkmark.Render() + card.name.Substring(4);
                }
                else if (card.name.IndexOf("[!]") == 0 || card.name.IndexOf("[!]") == 0)
                {
                    var checkmark = new Scaffold("/Views/Card/Kanban/Elements/warning.html");
                    scaffold["name"] = checkmark.Render() + card.name.Substring(4);
                }
                else
                {
                    scaffold["name"] = card.name;
                }
                
                scaffold["colors"] = "";

                //render custom design inside card container
                cardscaff["layout"] = scaffold.Render();
            }

            //load card container
            cardscaff["id"] = card.cardId.ToString();

            //render card container
            return cardscaff.Render();
        }

        public static Tuple<Query.Models.Card, string> Details(int boardId, int cardId)
        {
            try
            {
                var card = Query.Cards.GetDetails(boardId, cardId);
                var scaffold = new Scaffold("/Views/Card/Kanban/details.html");
                scaffold["list-name"] = card.listName;
                scaffold["description"] = card.description;
                scaffold["no-description"] = card.description.Length > 0 ? "hide" : "";
                scaffold["has-description"] = card.description.Length <= 0 ? "hide" : "";
                scaffold["archive-class"] = card.archived ? "hide" : "";
                scaffold["restore-class"] = card.archived ? "" : "hide";
                scaffold["delete-class"] = card.archived ? "" : "hide";
                return new Tuple<Query.Models.Card, string>(card, scaffold.Render());
            }
            catch (Exception)
            {
                throw new ServiceErrorException("Error loading card details");
            }
        }
    }
}
