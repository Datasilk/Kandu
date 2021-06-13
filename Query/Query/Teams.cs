using System.Collections.Generic;

namespace Query
{
    public static class Teams
    {
        public enum Roles
        {
            Member = 0, //can view/contribute to boards/lists/cards
            Moderator = 1, //moderates contributions made by members
            Manager = 2 //can create boards, invite members, assign roles, etc
        }

        public static int Create(Models.Team team, int ownerId)
        {
            return Sql.ExecuteScalar<int>(
                "Team_Create",
                new {team.orgId, ownerId, team.name, team.description }
            );
        }

        public static Models.Team GetTeam(int teamId)
        {
            var list = Sql.Populate<Models.Team>(
                "Team_Get",
                new { teamId }
            );
            if(list.Count > 0) { return list[0]; }
            return null;
        }

        public static void UpdateTeam(Models.Team team)
        {
            Sql.ExecuteNonQuery(
                "Team_Update",
                new { team.teamId, team.orgId, team.name, team.description }
            );
        }

        public enum SortList
        {
            teamId = 0,
            name = 1,
            dateCreated = 2
        }

        public static List<Models.Team>GetList(int orgId, int userId)
        {
            return Sql.Populate<Models.Team>("Teams_GetList", new { orgId, userId});
        }

        public static void UpdateMember(int teamId, int userId, Roles role)
        {
            Sql.ExecuteNonQuery("Team_UpdateMember", new { teamId, userId, roleId = (int)role });
        }
    }
}
