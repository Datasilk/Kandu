using System.Collections.Generic;
using System.Linq;

namespace Query
{
    public static class Security
    {
        public static void SaveKeys(int orgId, int userId, Dictionary<string, bool> keys)
        {
            //create class structure to use for XML
            var list = new Models.Xml.Keys()
            {
                List = keys.Select(a => new Models.Xml.Keys.Key() { Name = a.Key, Value = a.Value == true ? 1 : 0 }).ToArray()
            };
            Sql.ExecuteNonQuery("Security_SaveKeys", new { orgId, userId, keys = Common.Serializer.ToXmlDocument(list).OuterXml });
        }

        public static List<Models.SecurityKey>  ForUser (int orgId, int userId)
        {
            return Sql.Populate<Models.SecurityKey>("Security_ForUser", new { orgId, userId });
        }

        public static List<Models.SecurityKey> AllKeysForUser(int userId)
        {
            return Sql.Populate<Models.SecurityKey>("Security_AllKeysForUser", new { userId });
        }
    }
}
