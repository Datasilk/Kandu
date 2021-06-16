using System.Text;

namespace Kandu.Services
{
    public class Teams : Service
    {
        public string Create(int orgId, string name, string description = "")
        {
            if (!CheckSecurity(orgId, Common.Platform.Security.Keys.TeamCanCreate)) { return AccessDenied(); } //check security
            Common.Platform.Teams.Create(this, orgId, name, description);
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
            var html = Common.Platform.Teams.RenderList(this, orgId);
            if (CheckSecurity(orgId, Common.Platform.Security.Keys.TeamCanCreate))
            {
                var additem = new View("/Views/Teams/add-item.html");
                var addbutton = additem.Render();
                html = (btnsInFront == true ? addbutton : "") + html + (btnsInFront == false ? addbutton : "");
            }
            return html;
        }

        public string Details(int teamId)
        {
            var team = Query.Teams.GetTeam(teamId);
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var canEdit = CheckSecurity(team.orgId, Common.Platform.Security.Keys.TeamCanEditInfo);
            var tabHtml = new StringBuilder();
            var contentHtml = new StringBuilder();
            var view = new View("/Views/Teams/details.html");
            var tab = new View("/Views/Shared/tab.html");

            //load members tab
            tab["title"] = "Members";
            tab["id"] = "members";
            tab["onclick"] = "S.teams.details.tabs.select('members')";
            tab.Show("selected");
            tabHtml.Append(tab.Render());
            contentHtml.Append(Common.Platform.Members.RenderTeam(this, teamId));

            view["name"] = team.name;
            view["description"] = team.description;
            view["tabs"] = tabHtml.ToString();
            view["content"] = contentHtml.ToString();
            if (canEdit)
            {
                view.Show("can-edit");
            }
            else
            {
                view.Show("no-edit");
            }
            return view.Render();
        }

        public string Update(int teamId, string name, string description)
        {
            var team = Query.Teams.GetTeam(teamId);
            if (!CheckSecurity(team.orgId, Common.Platform.Security.Keys.TeamCanEditInfo)) { return AccessDenied(); } //check security
            Query.Teams.UpdateTeam(new Query.Models.Team()
            {
                teamId = teamId,
                orgId = team.orgId,
                name = name,
                description = description
            });
            return Success();
        }
    }
}
