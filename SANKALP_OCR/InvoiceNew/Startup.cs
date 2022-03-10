using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InvoiceNew.Startup))]
namespace InvoiceNew
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
