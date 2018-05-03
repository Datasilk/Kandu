using Microsoft.AspNetCore.Http;

namespace Kandu
{
    public class Service : Datasilk.Service
    {
        public Service(HttpContext context) : base(context) { }
    }
}