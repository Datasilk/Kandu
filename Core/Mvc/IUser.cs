using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Kandu.Core
{
    public interface IUser
    {
        int UserId { get; set; }
        int OrgId { get; set; }
        string VisitorId { get; set; }
        string Email { get; set; }
        string Name { get; set; }
        string DisplayName { get; set; }
        bool Photo { get; set; }
        DateTime DateCreated { get; set; }
        Dictionary<int, List<SecurityKey>> Keys { get; set; }
        bool ResetPass { get; set; } 
        bool KeepMenuOpen { get; set; }
        bool AllColor { get; set; }
        List<int> Boards { get; set; }

        IUser SetContext(HttpContext context);
        void LogIn(int userId, int orgId, string email, string name, DateTime datecreated, string displayName = "", bool photo = false);
        void LogOut();
        void Save(bool changed = false);

        bool CheckSecurity(int boardId);
        bool CheckSecurity(int orgId, string key, Models.Scope scope = Models.Scope.All, int scopeId = 0);
        bool IsInOrganization(int orgId);
        void ValidatePassword(string password);
    }
}
