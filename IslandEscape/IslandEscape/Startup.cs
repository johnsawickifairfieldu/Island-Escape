using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IslandEscape.Startup))]
namespace IslandEscape
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
