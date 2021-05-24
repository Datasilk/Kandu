using System.Text;

namespace Kandu.Services
{
    public class Teams : Service
    {
        public string Create(string name, string description = "")
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            
            return Success();
        }

        public string List(int orgId)
        {
            if (!IsInOrganization(orgId)) { return AccessDenied(); } //check security
            var list = Query.Teams.GetList(orgId, User.userId);
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

        public string RefreshList(int orgId)
        {
            if (!IsInOrganization(orgId)) { return AccessDenied(); } //check security
            return Common.Platform.Teams.RenderTeamsList(this, orgId);
        }
    }
}
