using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Query
{
    public class EmailActions
    {
        public static void Save(string action, string clientId, string label, string subject)
        {
            Sql.ExecuteNonQuery("EmailAction_Save", new { action, clientId, label, subject });
        }

        public static List<Models.EmailClientAction> GetList()
        {
            var list = Sql.Populate<Models.EmailClientAction>("EmailActions_GetList");
            if (list != null)
            {
                foreach(var item in list)
                {
                    item.config = JsonSerializer.Deserialize<Dictionary<string, string>>(item.config_json);
                }
                return list;
            }
            return null;
        }

        public static Models.EmailClientAction GetInfo(string action){

            var item = Sql.Populate<Models.EmailClientAction>("EmailAction_GetInfo", new { action }).FirstOrDefault();
            if(item != null)
            {
                item.config = JsonSerializer.Deserialize<Dictionary<string, string>>(item.config_json);
                return item;
            }
            return null;
        }
    }
}
