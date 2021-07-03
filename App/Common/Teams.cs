using System;
using System.Text;

namespace Kandu.Common
{
    public static class Teams
    {
        public static int Create(Core.IRequest request, int orgId, string name, string description = "")
        {
            try
            {
                return Query.Teams.Create(new Query.Models.Team()
                {
                    orgId = orgId,
                    name = name,
                    description = description
                }, request.User.UserId);
            }
            catch (Exception)
            {
                throw new ServiceErrorException("Error creating new team");
            }
        }

        public static string RenderList(Core.IRequest request, int orgId, bool showMembers = true)
        {
            var listItem = new View("/Views/Teams/list-item.html");
            var html = new StringBuilder();
            var teams = Query.Teams.GetList(orgId, request.User.UserId);
            foreach(var team in teams)
            {
                listItem.Clear();
                listItem.Bind(new { team });
                if (team.totalMembers != 1) { listItem.Show("plural"); }
                listItem["click"] = "S.orgs.teams.details(" + team.teamId + ", '" + team.name.Replace("'","\\'").Replace("\"", "&quot;") + "')";
                if (showMembers) { listItem.Show("subtitle"); }
                html.Append(listItem.Render());
            }

            return html.ToString();
        }

        public static string RenderMembers(Core.IRequest request, int teamId)
        {
            var listItem = new View("/Views/Members/list-item.html");
            var html = new StringBuilder();
            var members = Query.Teams.GetMembers(teamId);
            foreach (var member in members)
            {
                if(member.title == "") { member.title = "Member"; }
                listItem.Clear();
                listItem.Bind(new { member });
                listItem["click"] = "S.teams.member.show(" + member.userId + ")";
                html.Append(listItem.Render());
            }

            return html.ToString();
        }
    }
}
