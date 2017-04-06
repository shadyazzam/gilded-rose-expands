using Autofac;
using Autofac.Integration.WebApi;
using GildedRose.EFDataProvider;
using GildedRose.Logic;
using GildedRose.Providers;
using GildedRose.Repositories;
using System.Data.Common;
using System.Data.Entity;
using System.Reflection;
using System.Web.Http;
using Module = Autofac.Module;

namespace GildedRose
{
    /// <summary>
    /// Autofac Module for all registered components.
    /// </summary>
    /// <seealso cref="Autofac.Module" />
    public class WebApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);


            builder
                .RegisterModule<BusinessLogicModule>()
                .RegisterModule<EntityRepositoriesModule>();

            builder.RegisterType<UserService>().As<IUserService>();

            builder.Register(c => new DatabaseContext(c.Resolve<ConnectionStringBuilder>().ConnectionString)).As<DbContext>().InstancePerRequest();
        }
    }
}