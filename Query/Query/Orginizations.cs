using System.Collections.Generic;

namespace Query
{
    public static class Organizations
    {
        public static void Create(Models.Organization org)
        {
            Sql.ExecuteNonQuery("Organization_Create", new { org.ownerId, org.name, org.website, org.description, org.isprivate });
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
    }
}
