using Datasilk.Core.Web;

namespace Kandu
{
    public class Service : Request, IService
    {
        public string Success()
        {
            return "success";
        }

        public string Empty() { return "{}"; }
    }
}