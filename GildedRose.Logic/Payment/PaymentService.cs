using GildedRose.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Logic.Payment
{
    /// <summary>
    /// Service responsible for processing payments.
    /// </summary>
    public class PaymentService : IPaymentService
    {
        /// <summary>
        /// Processes a payment.
        /// </summary>
        /// <param name="amount">The amount to pay.</param>
        /// <param name="info">The payment info to process.</param>
        /// <returns></returns>
        public async Task<PaymentProcessResult> ProcessPayment(decimal amount, PaymentInfo info)
        {
            // Simulate an asynchronous payment processing operation
            return await Task.Run(() => {
                ValidatePayment(amount, info);

                // Performing real payment processing is out of the scope of this assignment - assume always successful
                return new PaymentProcessResult()
                {
                    Success = true,
                    ConfirmationCode = "WA12DF7JD2819ASD90Z"
                };
            });
        }

        /// <summary>
        /// Validates the amount and payment info.
        /// </summary>
        /// <param name="amount">The amount to pay.</param>
        /// <param name="info">The payment info to process.</param>
        public void ValidatePayment(decimal amount, PaymentInfo info)
        {
            if (amount <= 0)
                throw new ArgumentException("Payment amount must be greater than 0.");

            // Validation of PaymentInfo out of scope for this assignment - assume always valid
        }
    }
}
