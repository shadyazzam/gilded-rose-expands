using GildedRose.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Logic.Payment
{
    /// <summary>
    /// Defines the contract for a service responsible for processing payments.
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Processes a payment.
        /// </summary>
        /// <param name="amount">The amount to pay.</param>
        /// <param name="info">The payment info to process.</param>
        /// <returns></returns>
        Task<PaymentProcessResult> ProcessPayment(decimal amount, PaymentInfo info);

        /// <summary>
        /// Validates the amount and payment info.
        /// </summary>
        /// <param name="amount">The amount to pay.</param>
        /// <param name="info">The payment info to process.</param>
        void ValidatePayment(decimal amount, PaymentInfo info);
    }
}
