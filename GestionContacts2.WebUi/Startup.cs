using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GestionContacts2.WebUi.Startup))]
namespace GestionContacts2.WebUi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
