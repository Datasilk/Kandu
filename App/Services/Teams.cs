using System.Text;
using Microsoft.AspNetCore.Http;

namespace Kandu.Services
{
    public class Teams : Service
    {
        public Teams(HttpContext context) : base(context)
        {
        }

        public string Create(string name, string description = "")
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            
            return Success();
        }

        public string List()
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var query = new Query.Teams();
            var list = query.GetList(User.userId);
            var html = new StringBuilder("{\"teams\":[");
            var i = 0;
            list.ForEach((Query.Models.Team t) =>
            {
                html.Append((i > 0 ? "," : "") + "{\"name\":\"" + t.name + "\", \"description\":\"" + t.description + "\",\"teamId\":\"" + t.teamId + "\"}");
                i++;
            });
            html.Append("]}");
            return html.ToString();
        }
    }
}
