using System.Collections.Generic;

namespace Query
{
    public static class Orginizations
    {
        public static void Create(Models.Orginization org)
        {
            Sql.ExecuteNonQuery("Orginization_Create", new { org.ownerId, org.name, org.website, org.description, org.isprivate });
        }

        public static  void Disable(int orgId)
        {
            Sql.ExecuteNonQuery("Orginization_Disable", new { orgId });
        }

        public static void Enable(int orgId)
        {
            Sql.ExecuteNonQuery("Orginization_Enable", new { orgId });
        }

        public static Models.Orginization GetInfo(int orgId)
        {
            var list = Sql.Populate<Models.Orginization>("Orginization_GetInfo", new { orgId });
            if(list != null && list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        public static List<Models.Orginization> Owned(int ownerId)
        {
            return Sql.Populate<Models.Orginization>("Orginizations_Owned", new { ownerId });
        }

        public static List<Models.Orginization> UserIsPartOf(int userId)
        {
            return Sql.Populate<Models.Orginization>("Orginizations_UserIsPartOf", new { userId });
        }
    }
}
