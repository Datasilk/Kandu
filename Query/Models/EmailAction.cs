using System.Collections.Generic;

namespace Query.Models
{
    public class EmailAction
    {
        public string action { get; set; }
        public int clientId { get; set; }
        public string subject { get; set; }
        public string fromName { get; set; }
        public string fromAddress { get; set; }
        public string bodyText { get; set; }
        public string bodyHtml { get; set; }
    }

    public class EmailClientAction : EmailAction
    {
        public string key { get; set; } //email client key
        public string label { get; set; } //email client label
        public string config_json { get; set; } //email client config json
        public Dictionary<string, string> config { get; set; } //email client config
    }
}
