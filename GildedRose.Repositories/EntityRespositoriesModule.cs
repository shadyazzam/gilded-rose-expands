using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Repositories
{
    /// <summary>
    /// Autofac Module for the repositories.
    /// </summary>
    /// <seealso cref="Autofac.Module" />
    public class EntityRepositoriesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register base repository
            builder.RegisterGeneric(typeof(RepositoryBase<>)).As(typeof(IRepository<>));
        }
    }
}
