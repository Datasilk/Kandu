using System.Collections.Generic;

namespace Query.Models
{
    public class EmailAction
    {
        public string action { get; set; }
        public string clientId { get; set; }
        public string subject { get; set; }
        public string fromName { get; set; }
        public string fromAddress { get; set; }
    }

    public class EmailClientAction : EmailAction
    {
        public string key { get; set; }
        public string label { get; set; }
        public string config_json { get; set; }
        public Dictionary<string, string> config { get; set; }
    }
}
