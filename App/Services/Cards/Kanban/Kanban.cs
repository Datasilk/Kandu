using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var type = "default";
            var scaffold = LoadCardScaffold(type);
            scaffold.Data["id"] = card.cardId.ToString();
            scaffold.Data["title"] = card.name;
            scaffold.Data["colors"] = "";
            return scaffold.Render();
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
            var card = query.GetCardDetails(boardId, cardId);
            var scaffold = new Scaffold("/Services/Cards/Kanban/details.html", S.Server.Scaffold);
            return card.name + "|" + scaffold.Render();
        }
    }
}
