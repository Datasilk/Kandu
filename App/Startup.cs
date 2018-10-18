using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;


public class Startup : Datasilk.Startup {

    public override void Configured(IApplicationBuilder app, IHostingEnvironment env, IConfigurationRoot config)
    {
        base.Configured(app, env, config);
        Query.Sql.connectionString = server.sqlConnectionString;
        server.resetPass = Query.Users.HasPasswords();
        server.hasAdmin = Query.Users.HasAdmin();
    }
}
