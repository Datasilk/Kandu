namespace Kandu.Vendor
{
    public class SecurityKey
    {
        /// <summary>
        /// The value used when executing the method CheckSecurity(string key).
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// The label displayed in Kandu's security role manager UI.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// The description displayed in Kandu's security role manager UI.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A list of keys the user is required to have access to in order to
        /// be able to use this key when adding keys to a security group
        /// </summary>
        public string[] RequiredKeys { get; set; }
    }
}
