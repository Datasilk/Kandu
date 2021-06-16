using System;
using System.Text;

namespace Kandu.Common.Platform
{
    public static class Members
    {
        public static string RenderList(Request request, int orgId, bool showMembers = true)
        {
            var listItem = new View("/Views/Members/list-item.html");
            var html = new StringBuilder();
            var teams = Query.Teams.GetList(orgId, request.User.userId);
            foreach (var team in teams)
            {
                listItem.Clear();
                listItem.Bind(new { team });
                if (team.totalMembers != 1) { listItem.Show("plural"); }
                listItem["click"] = "S.orgs.member.show(" + team.teamId + ")";
                if (showMembers) { listItem.Show("subtitle"); }
                html.Append(listItem.Render());
            }

            return html.ToString();
        }

        public static string RenderTeam(Request request, int teamId)
        {
            var listItem = new View("/Views/Members/list-item.html");
            var html = new StringBuilder();
            var teams = Query.Teams.GetMembers(teamId);
            foreach (var team in teams)
            {
                listItem.Clear();
                listItem.Bind(new { team });
                if (team.totalMembers != 1) { listItem.Show("plural"); }
                listItem["click"] = "S.teams.member.show(" + team.teamId + ")";
                html.Append(listItem.Render());
            }

            return html.ToString();
        }
    }
}
