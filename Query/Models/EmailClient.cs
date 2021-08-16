using System.Collections.Generic;

namespace Query.Models
{
    public class EmailClient
    {
        public int clientId { get; set; }
        public string key { get; set; }
        public string label { get; set; }
        public string config_json { get; set; }
        public Dictionary<string, string> config { get; set; }
    }
}
