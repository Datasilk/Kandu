using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;


public class Startup : Datasilk.Startup {

    public override void Configured(IApplicationBuilder app, IHostingEnvironment env, IConfigurationRoot config)
    {
        base.Configured(app, env, config);
        var query = new Kandu.Query.Users(server.sqlConnectionString);
        server.resetPass = query.HasPasswords();
        server.hasAdmin = query.HasAdmin();
    }

    public override void Run(HttpContext context)
    {
        context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = 10_000_000;
        base.Run(context);
    }
}
