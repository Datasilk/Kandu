using System.Collections.Generic;

namespace Kandu.Vendor.Security
{
    public enum ScopeTypes
    {
        Organization = 1,
        SecurityGroup = 2,
        Team = 3,
        Board = 4,
        List = 5,
        Card = 6
    }

    public class RequiredKeys
    {
        /// <summary>
        /// Combines all application-wide security keys, organization-wide security keys, and any keys that you define into one array
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static RequiredKey[] Load(RequiredKey[] keys = null)
        {
            var newkeys = new List<RequiredKey>()
            {

                    new RequiredKey(){Key = "AppOwner" },
                    new RequiredKey(){Key = "AppFullAccess" },
                    new RequiredKey(){Key = "Owner", Scope = ScopeTypes.Organization },
                    new RequiredKey(){Key = "OrgFullAccess", Scope = ScopeTypes.Organization },
                    new RequiredKey(){Key = "SecGroupsFullAccess", Scope = ScopeTypes.SecurityGroup }
            };
            if(keys != null)
            {
                newkeys.AddRange(keys);
            }
            return newkeys.ToArray();
        }
    }

    public class RequiredKey
    {
        public string Key { get; set; }
        public ScopeTypes Scope { get; set; }
    }
}

namespace Kandu.Vendor { 
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
        /// Scope Types that are supported by the key
        /// </summary>
        public Security.ScopeTypes[] ScopeTypes { get; set; }

        /// <summary>
        /// A list of keys the user is required to have access to in order to
        /// be able to use this key when adding keys to a security group
        /// </summary>
        public Security.RequiredKey[] RequiredKeys { get; set; }
    }
}
