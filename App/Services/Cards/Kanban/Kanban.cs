using System.Collections.Generic;

namespace Kandu.Services.Card
{
    public class Kanban : Service
    {
        public Kanban(Core DatasilkCore) : base(DatasilkCore)
        {
        }

        private Dictionary<string, Scaffold> cards = new Dictionary<string, Scaffold>();

        public string LoadCardHtml(Query.Models.Card card)
        {
            if (!CheckSecurity()) { return AccessDenied(); }
            var type = "default";
            var cardscaff = new Scaffold("/Services/Cards/Kanban/card.html");

            //load card custom design
            var scaffold = LoadCardScaffold(type);
            scaffold.Data["title"] = card.name;
            scaffold.Data["colors"] = "";

            //load card container
            cardscaff.Data["id"] = card.cardId.ToString();
            
            //render custom design inside card container
            cardscaff.Data["layout"] = scaffold.Render();

            //render card container
            return cardscaff.Render();
        }

        private Scaffold LoadCardScaffold(string type)
        {
            if (cards.ContainsKey(type)) { return cards[type]; }
            var scaffold = new Scaffold("/Services/Cards/Kanban/Card/" + type + ".html");
            cards.Add(type, scaffold);
            return scaffold;
        }



        public string Details(int boardId, int cardId)
        {
            if (!UserInfo.CheckSecurity(boardId)) { return AccessDenied(); }
            var query = new Query.Cards(S.Server.sqlConnectionString);
            var card = query.GetDetails(boardId, cardId);
            var scaffold = new Scaffold("/Services/Cards/Kanban/details.html", S.Server.Scaffold);
            scaffold.Data["list-name"] = "";
            scaffold.Data["archive-class"] = card.archived ? "hide" : "";
            scaffold.Data["restore-class"] = card.archived ? "" : "hide";
            scaffold.Data["delete-class"] = card.archived ? "" : "hide";

            return card.name + "|" + scaffold.Render();
        }

        public string Move(int boardId, int listId, int cardId, int[] cardIds)
        {
            if (!UserInfo.CheckSecurity(boardId)) { return AccessDenied(); }
            var query = new Query.Cards(S.Server.sqlConnectionString);
            query.Move(boardId, listId, cardId, cardIds);
            return Success();
        }
    }
}
