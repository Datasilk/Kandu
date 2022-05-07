using System.Collections.Generic;

namespace Query
{
    public static class Organizations
    {
        public static int Create(Models.Organization org)
        {
            return Sql.ExecuteScalar<int>("Organization_Create", new { org.ownerId, org.name, org.website, org.description, org.isprivate, org.cardtype });
        }

        public static void Update(Models.Organization org)
        {
            Sql.ExecuteNonQuery("Organization_Update", new { org.orgId, org.name, org.website, org.description });
        }

        public static void UpdateSettings(int orgId, int groupId, string cardtype)
        {
            Sql.ExecuteNonQuery("Organization_UpdateSettings", new { orgId, groupId, cardtype });
        }

        public static  void Disable(int orgId)
        {
            Sql.ExecuteNonQuery("Organization_Disable", new { orgId });
        }

        public static void Enable(int orgId)
        {
            Sql.ExecuteNonQuery("Organization_Enable", new { orgId });
        }

        public static Models.Organization GetInfo(int orgId)
        {
            var list = Sql.Populate<Models.Organization>("Organization_GetInfo", new { orgId });
            if(list != null && list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        public static List<Models.Organization> Owned(int ownerId)
        {
            return Sql.Populate<Models.Organization>("Organizations_Owned", new { ownerId });
        }

        public static List<Models.Organization> UserIsPartOf(int userId)
        {
            return Sql.Populate<Models.Organization>("Organizations_UserIsPartOf", new { userId });
        }

        public static List<Models.Member> GetMembers(int orgId, int page = 1, int length = 10, string search = "", int? excludeTeamId = null)
        {
            return Sql.Populate<Models.Member>("Organization_GetMembers", new { orgId, page, length, search, excludeTeamId });
        }

        public static int GetMembersCount(int orgId, int page = 1, int length = 10, string search = "", int? excludeTeamId = null)
        {
            return Sql.ExecuteScalar<int>("Organization_GetMembersCount", new { orgId, page, length, search, excludeTeamId });
        }

        public static void UpdateCustomJs(int orgId, bool enabled)
        {
            Sql.ExecuteNonQuery("Organization_UpdateCustomJs", new { orgId, customJs = enabled });
        }

        public static void UpdateCustomCss(int orgId, bool enabled)
        {
            Sql.ExecuteNonQuery("Organization_UpdateCustomCss", new { orgId, customCss = enabled });
        }

    }
}
