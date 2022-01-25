namespace Kandu.Core
{
    public interface IRequest: Datasilk.Core.Web.IRequest
    {
        IUser User { get; set; }
        void AddScript(string url, string id = "", string callback = "");
        void AddCSS(string url, string id = "");
        bool CheckSecurity(int boardId);
        bool CheckSecurity(int orgId, string key, Models.Scope scope = Models.Scope.All, int scopeId = 0);
    }
}
