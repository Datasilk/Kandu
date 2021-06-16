using System.Collections.Generic;
using System.Linq;

namespace Query
{
    public static class Security
    {
        public static void SaveKeys(int orgId, int groupId, Dictionary<string, bool> keys)
        {
            //create class structure to use for XML
            var list = new Models.Xml.Keys()
            {
                List = keys.Select(a => new Models.Xml.Keys.Key() { Name = a.Key, Value = a.Value == true ? 1 : 0 }).ToArray()
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

        public static void UpdateKey(int orgId, int groupId, string key, string value)
        {
            Sql.ExecuteNonQuery("Security_UpdateKey", new { orgId, groupId, key, value });
        }
    }
}
