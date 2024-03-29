﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Strings;

namespace Kandu.Services
{
    public class Teams : Service
    {
        #region "Teams"
        public string List(int orgId)
        {
            if (!IsInOrganization(orgId)) { return AccessDenied(); } //check security
            var list = Query.Teams.GetList(orgId, User.UserId);
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
            var html = "<div class=\"grid-items\">" + Common.Teams.RenderList(this, orgId);
            if (CheckSecurity(orgId, Security.Keys.TeamCanCreate.ToString()))
            {
                var additem = new View("/Views/Teams/add-item.html");
                var addbutton = additem.Render();
                html = (btnsInFront == true ? addbutton : "") + html + (btnsInFront == false ? addbutton : "");
            }
            return html + "</div>";
        }
        #endregion

        #region "Team"

        public string RenderForm(int teamId, int orgId)
        {
            if (!IsInOrganization(orgId)) { return AccessDenied(); } //check security
            var view = new View("/Views/Teams/new-team.html");
            if (teamId != 0)
            {
                //Edit Team form
                var team = Query.Teams.GetTeam(teamId);
                if (!CheckSecurity(team.orgId, new string[] { Security.Keys.TeamCanEditInfo.ToString(), Security.Keys.TeamsFullAccess.ToString() }, Models.Scope.Team, teamId))
                {
                    return AccessDenied();
                }
                view["name"] = team.name;
                view["submit-label"] = "Update Team";
                view["submit-click"] = "S.teams.add.submit('" + teamId + "')";
            }
            else
            {
                //Create Team form
                if (!CheckSecurity(orgId, new string[] { Security.Keys.TeamCanCreate.ToString(), Security.Keys.TeamsFullAccess.ToString() }, Models.Scope.Team, teamId))
                {
                    return AccessDenied();
                }
                view["submit-label"] = "Create Team";
                view["submit-click"] = "S.teams.add.submit()";
            }
            return view.Render();
        }

        public string Create(int orgId, string name, string description = "")
        {
            if (!CheckSecurity(orgId, Security.Keys.TeamCanCreate.ToString())) { return AccessDenied(); } //check security
            Common.Teams.Create(this, orgId, name, description);
            return Success();
        }

        public string Details(int teamId)
        {
            //check security
            var team = Query.Teams.GetTeam(teamId);
            if (!CheckSecurity(team.orgId, new string[] { 
                Security.Keys.TeamCanView.ToString(), Security.Keys.TeamsCanViewAll.ToString(), Security.Keys.TeamsFullAccess.ToString() 
            }, Models.Scope.Team, teamId)) { return AccessDenied(); } 

            //view team details modal
            var canEdit = CheckSecurity(team.orgId, Security.Keys.TeamCanEditInfo.ToString(), Models.Scope.Team, teamId);
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
            if (team.groupId != null && team.groupId > 0) 
            { 
                html.Append(Common.Teams.RenderMembers(this, teamId));
            }
            if (CheckSecurity(team.orgId, Security.Keys.TeamCanInviteUsers.ToString(), Models.Scope.Team, teamId))
            {
                if (team.groupId != null && team.groupId > 0)
                {
                    var additem = new View("/Views/Members/add-item.html");
                    var addbutton = additem.Render();
                    html.Append(addbutton);
                }
                else
                {
                    html.Append("<div class=\"row pad text-center\"><p>Please set a default security group for your team before adding members.</p></div>");
                }
            }
            contentHtml.Append("<div class=\"content-members grid-items\">" + html.ToString() + "</div>");

            //load invites tab
            tab.Clear();
            tab["title"] = "Invites";
            tab["id"] = "invites";
            tab["onclick"] = "S.teams.details.tabs.select('invites')";
            tabHtml.Append(tab.Render());
            contentHtml.Append("<div class=\"content-invites pad-top\"></div>");

            //load settings tab
            tab.Clear();
            tab["title"] = "Settings";
            tab["id"] = "settings";
            tab["onclick"] = "S.teams.details.tabs.select('settings')";
            tabHtml.Append(tab.Render());
            contentHtml.Append("<div class=\"content-settings pad-top\"></div>");

            view["name"] = team.name;
            view["description"] = team.description;
            view["tabs"] = tabHtml.ToString();
            view["content"] = contentHtml.ToString();
            if (canEdit)
            {
                view.Show("can-edit");
            }
            return view.Render();
        }

        public string Update(int teamId, string name, string description)
        {
            //check security
            var team = Query.Teams.GetTeam(teamId);
            if (!CheckSecurity(team.orgId, new string[] {
                Security.Keys.TeamCanView.ToString(), Security.Keys.TeamCanEditInfo.ToString(), Security.Keys.TeamsFullAccess.ToString()
            }, Models.Scope.Team, teamId)) { return AccessDenied(); }

            //update team info
            Query.Teams.Update(new Query.Models.Team()
            {
                teamId = teamId,
                orgId = team.orgId,
                name = name,
                description = description
            });
            return Success();
        }

        public string RefreshMembers(int orgId, int teamId)
        {
            var team = Query.Teams.GetTeam(teamId);
            if (!CheckSecurity(team.orgId, new string[] {
                Security.Keys.TeamCanView.ToString(), Security.Keys.TeamsCanViewAll.ToString(), Security.Keys.TeamsFullAccess.ToString()
            }, Models.Scope.Team, teamId)) { return AccessDenied(); }
            var html = new StringBuilder("<div class=\"grid-items\">");
            html.Append(Common.Teams.RenderMembers(this, teamId));
            if (CheckSecurity(team.orgId, Security.Keys.TeamCanInviteUsers.ToString(), Models.Scope.Team, teamId))
            {
                if (team.groupId != null && team.groupId > 0)
                {
                    var additem = new View("/Views/Members/add-item.html");
                    var addbutton = additem.Render();
                    html.Append(addbutton);
                }
            }
            return html.ToString() + "</div>";
        }
        #endregion

        #region "Settings"

        public string RefreshSettings(int orgId, int teamId)
        {
            //check security
            var team = Query.Teams.GetTeam(teamId);
            if (!CheckSecurity(team.orgId, new string[] {
                Security.Keys.TeamCanView.ToString(), Security.Keys.TeamsCanViewAll.ToString(), Security.Keys.TeamsFullAccess.ToString()
            }, Models.Scope.Team, teamId)) { return AccessDenied(); }

            //view team settings
            var groups = Query.Security.GetGroups(orgId, User.UserId);
            var view = new View("/Views/Teams/settings.html");
            view.Bind(new { team });
            if(team.groupId == null || team.groupId <= 0)
            {
                view["groups"] = "<option value=\"0\">Select A Security Group...</option>\n";
            }
            view["groups"] += string.Join("\n", groups.Select(a => "<option value=\"" + a.groupId + "\"" + (team.groupId == a.groupId ? " selected" : "") + ">" + a.name + "</option>"));
            return view.Render();
        }

        public string SaveSettings(int orgId, int teamId, Dictionary<string, string> parameters)
        {
            var team = Query.Teams.GetTeam(teamId);
            if (!CheckSecurity(team.orgId, new string[] {
                Security.Keys.TeamCanEditSettings.ToString(), Security.Keys.TeamsFullAccess.ToString()
            }, Models.Scope.Team, teamId)) { return AccessDenied(); }

            var groupId = 0;
            if (parameters.ContainsKey("team_groupid")) int.TryParse(parameters["team_groupid"], out groupId);
            try
            {
                //save org settings
                Query.Teams.UpdateSettings(orgId, teamId, groupId);
            }
            catch (Exception)
            {
                return Error("Error saving team settings");
            }
            try
            {
                //send parameters to all related Partial Views 
                var data = new Dictionary<string, object>()
                {
                    {"orgId", orgId },
                    {"teamId", teamId }
                };
                Common.PartialViews.Save(this, parameters, Vendor.PartialViewKeys.Team_Settings, data);
            }
            catch (Exception)
            {
                return Error("Error saving plugin data for team settings");
            }
            return Success();
        }

        #endregion

        #region "Invite People"
        public string RenderInviteForm(int orgId, int teamId)
        {
            var team = Query.Teams.GetTeam(teamId);
            if (!CheckSecurity(team.orgId, new string[] {
                Security.Keys.TeamCanInviteUsers.ToString(), Security.Keys.TeamsFullAccess.ToString()
            }, Models.Scope.Team, teamId)) { return AccessDenied(); }
            //Invite People to Team form
            var view = new View("/Views/Teams/invite.html");
            if (!CheckSecurity(team.orgId, Security.Keys.TeamCanInviteUsers.ToString(), Models.Scope.Team, teamId) || orgId != team.orgId)
            {
                return AccessDenied();
            }
            view["name"] = team.name;
            view["submit-label"] = "Send Invites";
            view["submit-click"] = "S.teams.invite.submit('" + teamId + "')";
            return view.Render();
        }

        public string RefreshInviteList(int orgId, int teamId, int page = 1, int length = 10, string search = "")
        {
            var team = Query.Teams.GetTeam(teamId);
            if (!CheckSecurity(team.orgId, new string[] {
                Security.Keys.TeamCanInviteUsers.ToString(), Security.Keys.TeamsFullAccess.ToString()
            }, Models.Scope.Team, teamId)) { return AccessDenied(); }
            var m = new Members();
            m.Instantiate(this);
            return m.RefreshList(orgId, page, length, search, true, "Search", teamId, "search by name or email address", "S.teams.invite.select", "S.teams.invite.selectEmail");
        }

        public string RenderMemberSelectedItem(int orgId, int userId)
        {
            if (!IsInOrganization(orgId)) { return AccessDenied(); } //check security
            var view = new View("/Views/Members/list-item-selected.html");
            var member = Query.Users.GetInfo(userId);
            view.Bind(new { member });
            view["remove-onclick"] = "S.teams.invite.removeSelected(event, " + userId + ")";
            return view.Render();
        }

        public string RenderEmailSelectedItem(int orgId, string email)
        {
            if (!IsInOrganization(orgId)) { return AccessDenied(); } //check security
            var view = new View("/Views/Members/use-email-selected.html");
            view["email"] = email;
            view["remove-onclick"] = "S.teams.invite.removeSelectedEmail(event)";
            return view.Render();
        }

        public string InvitePeople(int orgId, int teamId, List<string> people, string message = "")
        {
            var team = Query.Teams.GetTeam(teamId);
            if (!CheckSecurity(team.orgId, new string[] {
                Security.Keys.TeamCanInviteUsers.ToString(), Security.Keys.TeamsFullAccess.ToString()
            }, Models.Scope.Team, teamId)) { return AccessDenied(); }

            //send each person an invite email to your team
            if (team.groupId.HasValue == false || team.groupId <= 0)
            {
                return Error("You must set a default security group for your team before inviting people to join");
            }
            try
            {
                var failed = Common.Invitations.Send(this, people, orgId, Models.Scope.Team, teamId, new string[] { "join" });
                if (failed.Count > 0)
                {
                    return Error(string.Join(",", failed.Select(a => a.name != a.email ? a.name : a.email).ToArray()));
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
            return Success();
        }
        #endregion
    }
}
