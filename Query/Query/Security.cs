using System.Collections.Generic;
using System.Linq;

namespace Query
{
    public static class Security
    {
        public static void SaveKeys(int orgId, int groupId, List<Models.SecurityKey> keys)
        {
            //create class structure to use for XML
            var list = new Models.Xml.Keys()
            {
                List = keys.Select(a => new Models.Xml.Keys.Key() { Name = a.key, Value = a.enabled == true ? 1 : 0, Scope = a.scope, ScopeId = a.scopeId }).ToArray()
            };
            Sql.ExecuteNonQuery("Security_SaveKeys", new { orgId, groupId, keys = Common.Serializer.ToXmlDocument(list).OuterXml });
        }

        public static List<Models.SecurityKey>  ForUser (int orgId, int userId)
        {
            return Sql.Populate<Models.SecurityKey>("Security_ForUser", new { orgId, userId });
        }

        public static List<Models.SecurityKey> AllKeysForUser(int userId)
        {
            return Sql.Populate<Models.SecurityKey>("Security_AllKeysForUser", new { userId });
        }

        public static void CreateGroup(int orgId, string name)
        {
            Sql.ExecuteNonQuery("Security_CreateGroup", new { orgId, name });
        }

        public static void UpdateGroup(int groupId, string name)
        {
            Sql.ExecuteNonQuery("Security_UpdateGroup", new { groupId, name });
        }

        public static void UpdateKey(int orgId, int groupId, string key, bool value, int scope = 0, int scopeId = 0)
        {
            Sql.ExecuteNonQuery("Security_UpdateKey", new { orgId, groupId, key, value, scope, scopeId });
        }

        public static void RemoveKey(int orgId, int groupId, string key, int scope = 0, int scopeId = 0)
        {
            Sql.ExecuteNonQuery("Security_RemoveKey", new { orgId, groupId, key, scope, scopeId });
        }

        public static List<Models.ScopeItem> GetScopeItems(int orgId, int groupId, string key, int scope = 0)
        {
            return Sql.Populate<Models.ScopeItem>("Security_GetScopeItems", new { orgId, groupId, key, scope });
        }

        public static List<Models.SecurityGroup> GetGroups(int orgId)
        {
            return Sql.Populate<Models.SecurityGroup>("SecurityGroups_GetList", new { orgId });
        }

        public static List<Models.SecurityGroup> GetGroups(int orgId, int spUserId)
        {
            return Sql.Populate<Models.SecurityGroup>("SecurityGroups_GetListForUser", new { orgId, spUserId, userId = 0 });
        }

        public static List<Models.SecurityGroup> GetGroupsForUser(int spUserId, int userId)
        {
            return Sql.Populate<Models.SecurityGroup>("SecurityGroups_GetListForUser", new { orgId = 0, spUserId, userId });
        }

        public static Models.SecurityGroup GroupInfo(int groupId)
        {
            return Sql.Populate<Models.SecurityGroup>("SecurityGroup_GetInfo", new { groupId }).FirstOrDefault();
        }

        public static Models.SecurityDetails GroupDetails(int groupId)
        {
            Models.SecurityDetails details;
            using(var conn = new Connection("SecurityGroup_GetDetails", new { groupId }))
            {
                var reader = conn.PopulateMultiple();
                details = reader.Read<Models.SecurityDetails>().FirstOrDefault();
                details.Keys = reader.Read<Models.SecurityKey>().ToList();
                details.Users = reader.Read<Models.User>().ToList();
                return details;
            }
        }
    }
}
