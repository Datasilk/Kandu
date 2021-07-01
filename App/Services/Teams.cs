using System.Text;

namespace Kandu.Services
{
    public class Teams : Service
    {
        public string Create(int orgId, string name, string description = "")
        {
            if (!CheckSecurity(orgId, Models.Security.Keys.TeamCanCreate)) { return AccessDenied(); } //check security
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
            var html = "<div class=\"grid-items\">" + Common.Platform.Teams.RenderList(this, orgId);
            if (CheckSecurity(orgId, Models.Security.Keys.TeamCanCreate))
            {
                var additem = new View("/Views/Teams/add-item.html");
                var addbutton = additem.Render();
                html = (btnsInFront == true ? addbutton : "") + html + (btnsInFront == false ? addbutton : "");
            }
            return html + "</div>";
        }

        public string RefreshMembers(int orgId, int teamId)
        {
            if (!IsInOrganization(orgId)) { return AccessDenied(); } //check security
            var team = Query.Teams.GetTeam(teamId);
            var html = new StringBuilder("<div class=\"grid-items\">");
            html.Append(Common.Platform.Teams.RenderMembers(this, teamId));
            if (CheckSecurity(team.orgId, Models.Security.Keys.TeamCanInviteUsers, Models.Security.Scope.Team, teamId))
            {
                var additem = new View("/Views/Members/add-item.html");
                var addbutton = additem.Render();
                html.Append(addbutton);
            }
            return html.ToString() + "</div>";
        }

        public string RenderForm(int teamId, int orgId)
        {
            if (!IsInOrganization(orgId)) { return AccessDenied(); } //check security
            var view = new View("/Views/Teams/new-team.html");
            if (teamId != 0)
            {
                //Create Team form
                var team = Query.Teams.GetTeam(teamId);
                if (!CheckSecurity(team.orgId, Models.Security.Keys.TeamCanCreate, Models.Security.Scope.Team, teamId))
                {
                    return AccessDenied();
                }
                view["name"] = team.name;
                view["submit-label"] = "Update Team";
                view["submit-click"] = "S.teams.add.submit('" + teamId + "')";
            }
            else
            {
                //Edit Team form
                view["submit-label"] = "Create Team";
                view["submit-click"] = "S.teams.add.submit()";
            }
            return view.Render();
        }

        public string RenderInviteForm(int orgId, int teamId)
        {
            if (!IsInOrganization(orgId)) { return AccessDenied(); } //check security
            //Invite People to Team form
            var view = new View("/Views/Teams/invite.html");
            var team = Query.Teams.GetTeam(teamId);
            if (!CheckSecurity(team.orgId, Models.Security.Keys.TeamCanInviteUsers, Models.Security.Scope.Team, teamId) || orgId != team.orgId)
            {
                return AccessDenied();
            }
            view["name"] = team.name;
            view["submit-label"] = "Send Invites";
            view["submit-click"] = "S.teams.members.add.submit('" + teamId + "')";
            return view.Render();
        }

        public string RefreshInviteList(int orgId, int teamId, int page = 1, int length = 10, string search = "")
        {
            if (!IsInOrganization(orgId)) { return AccessDenied(); } //check security
            var m = new Members();
            m.Instantiate(this);
            return m.RefreshList(orgId, page, length, search, true, "Search", teamId, "", "S.teams.members.add.selectEmail");
        }

        public string Details(int teamId)
        {
            if (!CheckSecurity()) { return AccessDenied(); } //check security
            var team = Query.Teams.GetTeam(teamId);
            var canEdit = CheckSecurity(team.orgId, Models.Security.Keys.TeamCanEditInfo, Models.Security.Scope.Team, teamId);
            var tabHtml = new StringBuilder();
            var contentHtml = new StringBuilder();
            var view = new View("/Views/Teams/details.html");
            var tab = new View("/Views/Shared/tab.html");
            var html = new StringBuilder();

            //load members tab
            tab["title"] = "Members";
            tab["id"] = "members";
            tab["onclick"] = "S.teams.details.tabs.select('members')";
            tab.Show("selected");
            tabHtml.Append(tab.Render());

            html.Append(Common.Platform.Teams.RenderMembers(this, teamId));
            if (CheckSecurity(team.orgId, Models.Security.Keys.TeamCanInviteUsers, Models.Security.Scope.Team, teamId))
            {
                var additem = new View("/Views/Members/add-item.html");
                var addbutton = additem.Render();
                html.Append(addbutton);
            }
            contentHtml.Append("<div class=\"content-members grid-items\">" + html.ToString() + "</div>");

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
            if (!CheckSecurity(team.orgId, Models.Security.Keys.TeamCanEditInfo, Models.Security.Scope.Team, teamId)) { return AccessDenied(); } //check security
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
