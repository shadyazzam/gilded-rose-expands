using Autofac;
using NSubstitute;
using System.Net.Http;
using System.Web.Http;

namespace GildedRose.Tests.Integration
{
    /// <summary>
    /// Sub-class of Startup enabling us to self-host the web application for integration tests.
    /// </summary>
    public class TestStartup : Startup
    {
        protected override void ConfigureIoC(ContainerBuilder builder)
        {
            // Get the connection string to use for the integration tests.
            ConnectionString = ConnectionStringBuilder.GetConnectionString();

            // Set up mocks.
            base.ConfigureIoC(builder);
        }
    }
}
