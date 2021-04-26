using System.Collections.Generic;

namespace Query
{
    public static class Teams
    {
        public static int CreateTeam(Models.Team team)
        {
            return Sql.ExecuteScalar<int>(
                "Team_Create",
                new {team.ownerId, team.name, team.description }
            );
        }

        public static Models.Team GetTeam(int teamId, int ownerId)
        {
            var list = Sql.Populate<Models.Team>(
                "Team_Get",
                new { teamId, ownerId }
            );
            if(list.Count > 0) { return list[0]; }
            return null;
        }

        public static void UpdateTeam(Models.Team team)
        {
            Sql.ExecuteNonQuery(
                "Team_Update",
                new { team.teamId, team.ownerId, team.name, team.description }
            );
        }

        public enum SortList
        {
            teamId = 0,
            name = 1,
            dateCreated = 2
        }

        public static List<Models.Team>GetList(int ownerId =0, int start = 1, int length = 20, string search = "", SortList orderBy = SortList.name)
        {
            return Sql.Populate<Models.Team>(
                "Teams_GetList",
                new { ownerId, start, length, search, orderby = (int)orderBy }
            );
        }
    }
}
