using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;


public class Startup : Datasilk.Startup {

    public override void Configured(IApplicationBuilder app, IHostingEnvironment env, IConfigurationRoot config)
    {
        base.Configured(app, env, config);
        //app.UseHttpsRedirection(); //force redirection to HTTPS
        Query.Sql.connectionString = Server.sqlConnectionString;
        Server.resetPass = Query.Users.HasPasswords();
        Server.hasAdmin = Query.Users.HasAdmin();
    }
}
