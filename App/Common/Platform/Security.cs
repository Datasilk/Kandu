namespace Kandu.Common.Platform
{
    public static class Security
    {
        public enum Keys
        {
            Owner, //owner of an organization
            User, //user of an organization
            OrgCanEdit, //edit organization name, website, description fields
            TeamsCanViewAll, //can view all teams for an organization
            TeamCanCreate, //can create teams for an organization
            TeamCanEditInfo, //can edit team info for an organization
            TeamCanAssignUser, //can assign users to teams
            TeamCanRemoveUser, //can remove users from a team
            TeamCanAssignRoles, //can assign roles to team members
            TeamCanAssignBoards, //can assign boards to team
            BoardsCanViewAll, //can view all boards in an organization
            BoardCanCreate, //can create boards for an organization
            BoardCanAssignUser, //can assign users to boards
            BoardCanRemoveUsers, //can remove users from boards they are assigned to
            BoardCanRemoveComment, //can remove comments
        }
    }
}
