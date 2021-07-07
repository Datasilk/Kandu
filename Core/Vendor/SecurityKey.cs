namespace Kandu.Vendor
{
    public class SecurityKey
    {
        /// <summary>
        /// The value used when executing the method CheckSecurity(string key).
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// The label displayed in Saber's security role manager UI.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// The description displayed in Saber's security role manager UI.
        /// </summary>
        public string Description { get; set; }
    }
}
