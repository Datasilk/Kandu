using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kandu.Vendor
{
    /// <summary>
    /// An interface used by vendors to extend the Startup class for Kandu
    /// </summary>
    public interface IVendorStartup
    {
        void ConfigureServices(IServiceCollection services) { }
        void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfigurationRoot config) { }
    }
}
