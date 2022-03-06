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
                RequiredKeys = new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = "AppOwner" }
                }
            },
            new Vendor.SecurityKey()
            {
                Label = "Application Full Access",
                Value = "AppFullAccess",
                Description = "Complete administrator access to the entire Kandu application",
                RequiredKeys = new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = "AppOwner" }
                }
            },
            new Vendor.SecurityKey()
            {
                Label = "Organization Owner",
                Value = "Owner",
                Description = "Owner of an organization with Administrator privilages",
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load()
            },

            new Vendor.SecurityKey()
            {
                Label = "User",
                Value = "User",
                Description = "A user of an organization with no permissions",
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load()
            },

            //feature specific security keys /////////////////////////////////////////////////
            new Vendor.SecurityKey()
            {
                Label = "Edit Organization Info",
                Value = "OrgCanEditInfo",
                Description = "Can edit an organization's name, website & description fields",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.Organization
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.OrgCanEditInfo.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Organization }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Edit Organization Settings",
                Value = "OrgCanEditSettings",
                Description = "Can edit an organization's settings",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.Organization
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.OrgCanEditSettings.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Organization }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Edit Organization Theme",
                Value = "OrgCanEditTheme",
                Description = "Can edit an organization's theme",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.Organization
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.OrgCanEditTheme.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Organization }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "View Member Email Address",
                Value = "OrgCanViewMemberEmailAddr",
                Description = "Can view an organization member's email address",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.Organization,
                    Kandu.Vendor.Security.ScopeTypes.SecurityGroup,
                    Kandu.Vendor.Security.ScopeTypes.Team
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.OrgCanViewMemberEmailAddr.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Organization }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Security Groups Full Access",
                Value = "SecGroupsFullAccess",
                Description = "Can create, view, & update all security groups in your organization",
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load()
            },
            new Vendor.SecurityKey()
            {
                Label = "View All Security Groups",
                Value = "SecGroupsCanViewAll",
                Description = "Can view all security groups for an organization",
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.SecGroupsCanViewAll.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Organization }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "View Security Group",
                Value = "SecGroupCanView",
                Description = "Can view a specific security group",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.SecurityGroup
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.SecGroupCanView.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.SecurityGroup }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Create Security Groups",
                Value = "SecGroupCanCreate",
                Description = "Can create new security groups",
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.SecGroupCanCreate.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Organization }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Edit Security Group Info",
                Value = "SecGroupCanEditInfo",
                Description = "Can edit information about a security group",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.SecurityGroup
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.SecGroupCanEditInfo.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.SecurityGroup }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Add Users To Security Groups",
                Value = "SecGroupCanAddUsers",
                Description = "Can add new users to a specific security group",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.SecurityGroup
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.SecGroupCanAddUsers.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.SecurityGroup }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Add Security Group Keys",
                Value = Security.Keys.SecGroupCanUpdateKeys.ToString(),
                Description = "Can add & update security keys for a specific security group",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.SecurityGroup
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.SecGroupCanUpdateKeys.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.SecurityGroup }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Remove Security Group Users",
                Value = "SecGroupCanRemoveUsers",
                Description = "Can remove users from a specific security group",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.SecurityGroup
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.SecGroupCanRemoveUsers.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.SecurityGroup }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Teams Full Access",
                Value = "TeamsFullAccess",
                Description = "Can create, view, & update all teams in your organization",
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.TeamsFullAccess.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Organization }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "View All Teams",
                Value = "TeamsCanViewAll",
                Description = "Can view all teams in your organization",
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.TeamsCanViewAll.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Organization }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "View Team",
                Value = "TeamCanView",
                Description = "Can view a specific team in your organization",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.Team
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.TeamCanView.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Team }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Create Teams",
                Value = "TeamCanCreate",
                Description = "Can create new teams in your organization",
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.TeamCanCreate.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Organization }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Edit Team Info",
                Value = "TeamCanEditInfo",
                Description = "Can edit information about a specific team",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.Team
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.TeamCanEditInfo.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Team }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Edit Team Settings",
                Value = "TeamCanEditSettings",
                Description = "Can edit settings for a specific team",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.Team
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.TeamCanEditSettings.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Team }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Invite Users To Team",
                Value = "TeamCanInviteUsers",
                Description = "Can invite users to join a specific team",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.Team
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.TeamCanInviteUsers.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Team }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Remove Users From Team",
                Value = "TeamCanRemoveUsers",
                Description = "Can remove users from a specific team",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.Team
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.TeamCanRemoveUsers.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Team }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Assign Roles To Team Users",
                Value = "TeamCanAssignRoles",
                Description = "Can assign a role to users from a specific team",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.Team
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.TeamCanAssignRoles.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Team }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Assign Boards To Team Users",
                Value = "TeamCanAssignBoards",
                Description = "Can assign boards to users from a specific team",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.Team
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.TeamCanAssignBoards.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Team }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Boards Full Access",
                Value = "BoardsFullAccess",
                Description = "Can create, view, & update all boards in your organization",
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.BoardsFullAccess.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Organization }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "View All Boards",
                Value = "BoardsCanViewAll",
                Description = "Can view all boards in your organization",
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.BoardsCanViewAll.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Organization }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "View Board",
                Value = "BoardCanView",
                Description = "Can view a specific board in your organization",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.Board
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.BoardCanView.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Board }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Create Boards",
                Value = "BoardCanCreate",
                Description = "Can create new boards in your organization",
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.BoardCanCreate.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Organization }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Update Board",
                Value = "BoardCanUpdate",
                Description = "Can update cards & lists for a specific board",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.Board
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.BoardCanUpdate.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Board }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Remove Board Comments",
                Value = "BoardCanRemoveComment",
                Description = "Can remove any comment created in a specific board",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.Board
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.BoardCanRemoveComments.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Board }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "View Card",
                Value = "CardCanView",
                Description = "Can view a specific card",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.Board
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.CardCanView.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Card }
                })
            },
            new Vendor.SecurityKey()
            {
                Label = "Update Card",
                Value = "CardCanUpdate",
                Description = "Can update a specific card",
                ScopeTypes = new Vendor.Security.ScopeTypes[]
                {
                    Kandu.Vendor.Security.ScopeTypes.Board
                },
                RequiredKeys = Kandu.Vendor.Security.RequiredKeys.Load(new Vendor.Security.RequiredKey[]{
                    new Vendor.Security.RequiredKey(){Key = Security.Keys.CardCanUpdate.ToString(), Scope = Kandu.Vendor.Security.ScopeTypes.Card }
                })
            }
        };
    }
}
