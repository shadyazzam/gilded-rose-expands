using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Autofac;
using System.Web.Http;
using Autofac.Integration.WebApi;

[assembly: OwinStartup(typeof(GildedRose.Startup))]

namespace GildedRose
{
    public partial class Startup
    {
        public string ConnectionString { get; set; }

        public void Configuration(IAppBuilder app)
        {            
            // IOC Container Setup
            var builder = new ContainerBuilder();

            HttpConfiguration httpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(httpConfiguration);

            ConfigureIoC(builder);

            // Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(httpConfiguration);
            builder.Register(c => new ConnectionStringBuilder { ConnectionString = ConnectionString });

            builder.RegisterModule<WebApiModule>();
            
            var container = builder.Build();

            ConfigureAuth(app, container);

            // Set the dependency resolver to be Autofac.
            httpConfiguration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseWebApi(httpConfiguration);
        }

        protected virtual void ConfigureIoC(ContainerBuilder builder)
        {

        }
    }
}
