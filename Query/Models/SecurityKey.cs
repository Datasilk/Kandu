namespace Query.Models
{
    public class SecurityKey
    {
        public int orgId { get; set; }
        public int userId { get; set; }
        public string key { get; set; }
        public bool enabled { get; set; }
    }
}
