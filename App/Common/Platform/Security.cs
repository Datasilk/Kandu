using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kandu.Common.Platform
{
    public static class Security
    {
        public enum Keys
        {
            OrgCanEdit, //edit organization name, website, description fields
            BoardCanCreate, //can create boards for an organization
            BoardCanAssignUser, //can assign users to boards
            BoardCanRemoveUsers, //can remove users from boards they are assigned to
            BoardCanRemoveComment, //can remove comments
        }
    }
}
