using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(MvcRepules.Areas.Identity.IdentityHostingStartup))]
namespace MvcRepules.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}