using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Kandu
{
    public class Service
    {
        protected Core S;
        public Dictionary<string, string> Form = new Dictionary<string, string>();
        public IFormFileCollection Files;

        public Service(Core KanduCore) {
            S = KanduCore;
        }

        public string AccessDenied()
        {
            return "access denied";
        }

        public string Success()
        {
            return "success";
        }

        public string Error()
        {
            return "error";
        }
    }
}
