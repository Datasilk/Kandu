namespace Kandu.Core
{
    public interface IService : Datasilk.Core.Web.IService
    {
        Service Instantiate(IRequest request);
        string JsonResponse(dynamic obj);
        bool IsInOrganization(int orgId);
    }
}