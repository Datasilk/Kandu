using Kandu.Vendor;

namespace Kandu.Common
{
    public class SecurityKeys : IVendorKeys
    {
        public string Vendor { get; set; } = "Kandu";
        public Vendor.SecurityKey[] Keys { get; set; } = new Vendor.SecurityKey[]
        {
            //user type security keys //////////////////////////////////////////////////////
            new Vendor.SecurityKey()
            {
                Label = "Application Owner",
                Value = "AppOwner",
                Description = "Owner of the Kandu application with full, site-wide Administrator privilages",
            },
            new Vendor.SecurityKey()
            {
                Label = "Organization Owner",
                Value = "Owner",
                Description = "Owner of an organization with Administrator privilages"
            },
            new Vendor.SecurityKey()
            {
                Label = "User",
                Value = "User",
                Description = "A user of an organization with no permissions"
            },

            //feature specific security keys /////////////////////////////////////////////////
            new Vendor.SecurityKey()
            {
                Label = "Edit Organization Info",
                Value = "OrgCanEditInfo",
                Description = "Can edit an organization's name, website & description fields"
            },
            new Vendor.SecurityKey()
            {
                Label = "Edit Organization Settings",
                Value = "OrgCanEditSettings",
                Description = "Can edit an organization's settings"
            },
            new Vendor.SecurityKey()
            {
                Label = "Edit Organization Theme",
                Value = "OrgCanEditTheme",
                Description = "Can edit an organization's theme"
            },
            new Vendor.SecurityKey()
            {
                Label = "View Member Email Address",
                Value = "OrgCanViewMemberEmailAddr",
                Description = "Can view an organization member's email address"
            },
            new Vendor.SecurityKey()
            {
                Label = "View All Security Groups",
                Value = "SecGroupsCanViewAll",
                Description = "Can view all security groups for an organization"
            },
            new Vendor.SecurityKey()
            {
                Label = "View Security Group",
                Value = "SecGroupCanView",
                Description = "Can view a specific security group"
            },
            new Vendor.SecurityKey()
            {
                Label = "Create Security Groups",
                Value = "SecGroupCanCreate",
                Description = "Can create new security groups"
            },
            new Vendor.SecurityKey()
            {
                Label = "Edit Security Group Info",
                Value = "SecGroupCanEditInfo",
                Description = "Can edit information about a security group"
            },
            new Vendor.SecurityKey()
            {
                Label = "Add Users To Security Groups",
                Value = "SecGroupCanAddUsers",
                Description = "Can add new users to a specific security group"
            },
            new Vendor.SecurityKey()
            {
                Label = "Add Security Group Keys",
                Value = "SecGroupCanUpdateKeys",
                Description = "Can add & update security keys for a specific security group"
            },
            new Vendor.SecurityKey()
            {
                Label = "Remove Security Group Users",
                Value = "SecGroupCanRemoveUsers",
                Description = "Can remove users from a specific security group"
            },
            new Vendor.SecurityKey()
            {
                Label = "View All Teams",
                Value = "TeamsCanViewAll",
                Description = "Can view all teams for a specific organization"
            },
            new Vendor.SecurityKey()
            {
                Label = "View Team",
                Value = "TeamCanView",
                Description = "Can view a specific team in your organization"
            },
            new Vendor.SecurityKey()
            {
                Label = "Create Teams",
                Value = "TeamCanCreate",
                Description = "Can create new teams in your organization"
            },
            new Vendor.SecurityKey()
            {
                Label = "Edit Team Info",
                Value = "TeamCanEditInfo",
                Description = "Can edit information about a specific team"
            },
            new Vendor.SecurityKey()
            {
                Label = "Edit Team Settings",
                Value = "TeamCanEditSettings",
                Description = "Can edit settings for a specific team"
            },
            new Vendor.SecurityKey()
            {
                Label = "Invite Users To Team",
                Value = "TeamCanInviteUsers",
                Description = "Can invite users to join a specific team"
            },
            new Vendor.SecurityKey()
            {
                Label = "Remove Users From Team",
                Value = "TeamCanRemoveUsers",
                Description = "Can remove users from a specific team"
            },
            new Vendor.SecurityKey()
            {
                Label = "Assign Roles To Team Users",
                Value = "TeamCanAssignRoles",
                Description = "Can assign a role to users from a specific team"
            },
            new Vendor.SecurityKey()
            {
                Label = "Assign Boards To Team Users",
                Value = "TeamCanAssignBoards",
                Description = "Can assign boards to users from a specific team"
            },
            new Vendor.SecurityKey()
            {
                Label = "View All Boards",
                Value = "BoardsCanViewAll",
                Description = "Can view all boards in your organization"
            },
            new Vendor.SecurityKey()
            {
                Label = "View Board",
                Value = "BoardCanView",
                Description = "Can view a specific board in your organization"
            },
            new Vendor.SecurityKey()
            {
                Label = "Create Boards",
                Value = "BoardCanCreate",
                Description = "Can create new boards in your organization"
            },
            new Vendor.SecurityKey()
            {
                Label = "Update Board",
                Value = "BoardCanUpdate",
                Description = "Can update cards & lists for a specific board"
            },
            new Vendor.SecurityKey()
            {
                Label = "Remove Board Comments",
                Value = "BoardCanRemoveComment",
                Description = "Can remove any comment created in a specific board"
            }
        };
    }
}
