using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MCCard.Startup))]
namespace MCCard
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
