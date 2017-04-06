using GildedRose.Logic.Payment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace GildedRose.Tests.Unit.Services
{
    [TestClass]
    public class PaymentServiceTest
    {
        [TestMethod]
        [TestCategory("ServiceTests")]
        public async Task ProcessPayment_ShouldReturnConfirmationNumber()
        {
            decimal amount = 10;
            var paymentInfo = new PaymentInfo()
            {
                Card = CreditCard.Visa,
                CardHolderName = "John Doe",
                CardNumber = 1234567891234567,
                CardSecurityCode = 123,
                ExpiryMonth = 1,
                ExpiryYear = 2017,
                BillingAddress = "Billing Address"
            };

            var paymentService = new PaymentService();
            var paymentProcessResult = await paymentService.ProcessPayment(amount, paymentInfo);

            Assert.IsTrue(paymentProcessResult.Success);
            Assert.IsNotNull(paymentProcessResult.ConfirmationCode);
            Assert.IsNull(paymentProcessResult.ErrorMessage);
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        public void ValidatePayment_ValidAmount_ShouldNotThrowException()
        {
            decimal amount = 10;
            var paymentInfo = new PaymentInfo()
            {
                Card = CreditCard.Visa,
                CardHolderName = "John Doe",
                CardNumber = 1234567891234567,
                CardSecurityCode = 123,
                ExpiryMonth = 1,
                ExpiryYear = 2017,
                BillingAddress = "Billing Address"
            };

            var paymentService = new PaymentService();
            paymentService.ValidatePayment(amount, paymentInfo);
        }

        [TestMethod]
        [TestCategory("ServiceTests")]
        [ExpectedException(typeof(ArgumentException), "Payment amount must be greater than 0.")]
        public void ValidatePayment_InvalidAmount_ShouldThrowException()
        {
            decimal amount = 0;
            var paymentInfo = new PaymentInfo()
            {
                Card = CreditCard.Visa,
                CardHolderName = "John Doe",
                CardNumber = 1234567891234567,
                CardSecurityCode = 123,
                ExpiryMonth = 1,
                ExpiryYear = 2017,
                BillingAddress = "Billing Address"
            };

            var paymentService = new PaymentService();
            paymentService.ValidatePayment(amount, paymentInfo);
        }
    }
}
