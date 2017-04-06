using Autofac;
using GildedRose.Logic.Payment;

namespace GildedRose.Logic
{
    /// <summary>
    /// Autofac Module for business logic layer (services).
    /// </summary>
    /// <seealso cref="Autofac.Module" />
    public class BusinessLogicModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ItemService>().As<IItemService>();
            builder.RegisterType<PurchaseService>().As<IPurchaseService>();
            builder.RegisterType<PaymentService>().As<IPaymentService>();
        }
    }
}
