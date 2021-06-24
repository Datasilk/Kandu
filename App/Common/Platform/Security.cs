﻿using System.Text;

namespace Kandu.Common.Platform
{
    public static class Security
    {

        public enum Scope
        {
            All = 0,
            Organization = 1,
            SecurityGroup = 2,
            Team = 3,
            Board = 4,
            List = 5,
            Card = 6
        }

        public enum Keys
        {
            Owner, //owner of an organization
            User, //user of an organization
            OrgCanEdit, //edit organization name, website, description fields
            SecGroupsCanViewAll, //can view all security groups for an organization
            SecGroupCanCreate, //can create a new security group for an organization
            SecGroupCanEditInfo, //can update the name of name security groups for an organization
            SecGroupCanAddUsers, //can add users to security groups for an organization
            SecGroupCanUpdateKeys, //can update security keys for security groups within an organization
            SecGroupCanRemoveUsers, //can remove users from a security group
            TeamsCanViewAll, //can view all teams for an organization
            TeamCanCreate, //can create teams for an organization
            TeamCanEditInfo, //can edit team info for an organization
            TeamCanInviteUsers, //can invite users to teams
            TeamCanRemoveUsers, //can remove users from a team
            TeamCanAssignRoles, //can assign roles to team members
            TeamCanAssignBoards, //can assign boards to team
            BoardsCanViewAll, //can view all boards in an organization
            BoardCanCreate, //can create boards for an organization
            BoardCanUpdate, //can update boards for an organization
            BoardCanAssignUser, //can assign users to boards
            BoardCanRemoveUsers, //can remove users from boards they are assigned to
            BoardCanRemoveComment, //can remove comments
        }

        public static string RenderList(Request request, int orgId)
        {
            var listItem = new View("/Views/Security/list-item.html");
            var html = new StringBuilder();
            var groups = Query.Security.GetGroups(orgId);
            foreach (var group in groups)
            {
                listItem.Clear();
                listItem.Bind(new { group });
                if (group.keys != 1) { listItem.Show("plural"); }
                listItem["click"] = "S.orgs.teams.details(" + group.groupId + ", '" + group.name.Replace("'", "\\'").Replace("\"", "&quot;") + "')";
                listItem.Show("subtitle");
                html.Append(listItem.Render());
            }

            return html.ToString();
        }
    }
}
