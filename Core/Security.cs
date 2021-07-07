using System.Collections.Generic;

namespace Kandu
{
    public class Security
    {

        public enum Keys
        {
            Owner, //owner of an organization
            User, //user of an organization
            OrgCanEditInfo, //edit organization name, website, description fields
            OrgCanEditSettings, //edit organization settings
            OrgCanEditTheme, //edit organization theme
            SecGroupsCanViewAll, //can view all security groups for an organization
            SecGroupCanView, //can view a specific security group for an organization (uses scope & scopeId)
            SecGroupCanCreate, //can create a new security group for an organization
            SecGroupCanEditInfo, //can update the name of name security groups for an organization
            SecGroupCanAddUsers, //can add users to security groups for an organization
            SecGroupCanUpdateKeys, //can update security keys for security groups within an organization
            SecGroupCanRemoveUsers, //can remove users from a security group
            TeamsCanViewAll, //can view all teams for an organization
            TeamCanView, //can view a specific team for an organization (uses scope & scopeId)
            TeamCanCreate, //can create teams for an organization
            TeamCanEditInfo, //can edit team info for an organization
            TeamCanEditSettings, //can edit team settings for an organization
            TeamCanInviteUsers, //can invite users to teams
            TeamCanRemoveUsers, //can remove users from a team
            TeamCanAssignRoles, //can assign roles to team members
            TeamCanAssignBoards, //can assign boards to team
            BoardsCanViewAll, //can view all boards in an organization
            BoardCanView, //can view a specific board in an organization (uses scope & scopeId)
            BoardCanCreate, //can create boards for an organization
            BoardCanUpdate, //can update boards for an organization
            BoardCanAssignUser, //can assign users to boards
            BoardCanRemoveUsers, //can remove users from boards they are assigned to
            BoardCanRemoveComment, //can remove comments
        }
    }

    public class SecurityKeys
    {
        public int GroupId { get; set; }
        public int OrgId { get; set; }
        public List<SecurityKey> Keys { get; set; }
    }

    public class SecurityKey
    {
        public string Key { get; set; }
        public bool Enabled { get; set; }
        public Models.Scope Scope { get; set; } = 0;
        public int ScopeId { get; set; } = 0;
    }
}
