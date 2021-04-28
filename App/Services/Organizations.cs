using System.Text;

namespace Kandu.Services
{
    public class Organizations : Service
    {
        public string Create(string name, string description = "")
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security

            return Success();
        }

        public string List(string security = "")
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var list = Query.Organizations.UserIsPartOf(User.userId);
            var html = new StringBuilder("{\"orgs\":[");
            var i = 0;
            list.ForEach((Query.Models.Organization o) =>
            {
                if(security == "" || (security != "" && User.CheckSecurity(o.orgId, security)))
                {
                    html.Append((i > 0 ? "," : "") + "{\"name\":\"" + o.name + "\", \"description\":\"" + o.description + "\",\"orgId\":\"" + o.orgId + "\"}");
                }
                i++;
            });
            html.Append("]}");
            return html.ToString();
        }
    }
}
