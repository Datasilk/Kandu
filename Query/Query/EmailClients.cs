using System.Collections.Generic;
using System.Text.Json;

namespace Query
{
    public static class EmailClients
    {
        public static Models.EmailClient GetConfig(string clientId)
        {
            var list = Sql.Populate<Models.EmailClient>("EmailClient_GetConfig", new { clientId });
            foreach (var item in list)
            {
                item.config = JsonSerializer.Deserialize<Dictionary<string, string>>(item.config_json);
                return item;
            }
            return null;
        }

        public static List<Models.EmailClient> GetList()
        {
            var list = Sql.Populate<Models.EmailClient>("EmailClients_GetList");
            foreach(var item in list)
            {
                item.config = JsonSerializer.Deserialize<Dictionary<string, string>>(item.config_json);
            }
            return list;
        }

        public static void Save(string clientId, string key, string label, Dictionary<string, string> config)
        {
            Sql.ExecuteNonQuery("EmailClient_Save", new { clientId, key, label, config = JsonSerializer.Serialize(config) });
        }

        public static void Remove(string clientId)
        {
            Sql.ExecuteNonQuery("EmailClient_Remove", new { clientId });
        }
    }
}
