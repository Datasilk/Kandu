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

        public string RefreshList(int orgId, bool btnsInFront = false)
        {
            if (!IsInOrganization(orgId)) { return AccessDenied(); } //check security
            var html =  Common.Platform.Teams.RenderTeamsList(this, orgId);
            if(CheckSecurity(orgId, Common.Platform.Security.Keys.TeamCanCreate))
            {
                var additem = new View("/Views/Teams/add-item.html");
                var addbutton = additem.Render();
                html = (btnsInFront == true ? addbutton : "") + html + (btnsInFront == false ? addbutton : "");
            }
            return html;
        }
    }
}
