using System;
using System.Text;

namespace Kandu.Common.Platform
{
    public static class Teams
    {
        public static int Create(Request request, int orgId, string name, string description = "")
        {
            try
            {
                return Query.Teams.Create(new Query.Models.Team()
                {
                    orgId = orgId,
                    name = name,
                    description = description
                }, request.User.userId);
            }
            catch (Exception)
            {
                throw new ServiceErrorException("Error creating new team");
            }
        }

        public static string RenderTeamsList(Request request, int orgId, bool showMembers = true)
        {
            var listItem = new View("/Views/Teams/list-item.html");
            var html = new StringBuilder();
            var teams = Query.Teams.GetList(orgId, request.User.userId);
            foreach(var team in teams)
            {
                listItem.Clear();
                listItem.Bind(new { team });
                if (team.totalMembers != 1) { listItem.Show("plural"); }
                listItem["click"] = "S.orgs.team.show(" + team.teamId + ")";
                if (showMembers) { listItem.Show("subtitle"); }
                html.Append(listItem.Render());
            }

            return html.ToString();
        }
    }
}
