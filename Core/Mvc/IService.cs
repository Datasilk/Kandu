namespace Kandu.Core
{
    public interface IService : Datasilk.Core.Web.IService
    {
        Service Instantiate(IRequest request);
        string JsonResponse(dynamic obj);
        bool IsInOrganization(int orgId);
        bool CheckSecurity();
        bool CheckSecurity(int boardId);
        bool CheckSecurity(int orgId, string key, Models.Scope scope = Models.Scope.All, int scopeId = 0);
    }
}