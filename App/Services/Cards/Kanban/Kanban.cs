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
            scaffold.Data["title"] = card.name;
            scaffold.Data["colors"] = "";
            return scaffold.Render();
        }

        private Scaffold LoadCardScaffold(string type)
        {
            if (cards.ContainsKey(type)) { return cards[type]; }
            var scaffold = new Scaffold("/Services/Cards/Kanban/" + type + ".html");
            cards.Add(type, scaffold);
            return scaffold;
        }
    }
}
