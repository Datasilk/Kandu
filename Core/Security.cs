namespace Kandu.Core
{
    public class Security
    {
        public string Key { get; set; }
        public bool Enabled { get; set; }
        public Models.Scope Scope { get; set; } = 0;
        public int ScopeId { get; set; } = 0;
    }
}
