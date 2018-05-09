using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;


public class Startup : Datasilk.Startup {

    public override void Configured(IApplicationBuilder app, IHostingEnvironment env, IConfigurationRoot config)
    {
        base.Configured(app, env, config);
        Kandu.Query.QuerySql.connectionString = server.sqlConnectionString;
        var query = new Kandu.Query.Users();
        server.resetPass = query.HasPasswords();
        server.hasAdmin = query.HasAdmin();
    }
}
