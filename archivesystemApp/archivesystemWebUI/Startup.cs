using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(archivesystemWebUI.Startup))]
namespace archivesystemWebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
