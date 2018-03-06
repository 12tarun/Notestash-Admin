using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Notestash_Admin.Startup))]
namespace Notestash_Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
