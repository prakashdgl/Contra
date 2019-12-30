using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Contra.Areas.Identity.IdentityHostingStartup))]
namespace Contra.Areas.Identity
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