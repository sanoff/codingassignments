using System;
using Xunit;
using QuadPay.Domain;

namespace QuadPay.Test
{
    public class DomainTests
    {

        [Theory]
        [InlineData(-100, 4, 2)]
        [InlineData(123.23, 0, 2)]
        [InlineData(100, 4, 0)]
        [InlineData(100, -2, -3)]
        [InlineData(0, 2, 4)]
        // TODO What other situations should we be testing?
        public void ShouldThrowExceptionForInvalidParameters(decimal amount, int installmentCount, int installmentIntervalDays)
        {
            Assert.Throws<ArgumentException>(() => {
                var paymentPlan = new PaymentPlan(amount, installmentCount, installmentIntervalDays);
            });
        }

        [Theory]
        [InlineData(1000, 4, 2)]
        [InlineData(123.23, 2, 2)]
        [InlineData(325, 2, 1)]
        [InlineData(200, 1, 1)]
        [InlineData(200, 50, 1)]
        [InlineData(200, 1, 50)]
        // TODO What other situations should we be testing?
        public void ShouldCreateCorrectNumberOfInstallments(decimal amount, int installmentCount, int installmentIntervalDays)
        {
            var paymentPlan = new PaymentPlan(amount, installmentCount, installmentIntervalDays);
            Assert.Equal(installmentCount, paymentPlan.Installments.Count);
        }

        [Fact]
        public void ShouldReturnCorrectAmountToRefundAgainstPaidInstallments() {
            var paymentPlan = new PaymentPlan(100, 4);
            var firstInstallment = paymentPlan.FirstInstallment();
            paymentPlan.MakePayment(25, firstInstallment.Id);
            var cashRefundAmount = paymentPlan.ApplyRefund(new Refund(Guid.NewGuid().ToString(), 100));
            Assert.Equal(25, cashRefundAmount);
            Assert.Equal(0, paymentPlan.OustandingBalance());
        }

        [Fact]
        public void ShouldReturnCorrectOutstandingBalance() {
            var paymentPlan = new PaymentPlan(100, 4);
            var firstInstallment = paymentPlan.FirstInstallment();
            paymentPlan.MakePayment(25, firstInstallment.Id);
            var secondInstallment = paymentPlan.NextInstallment();
            paymentPlan.MakePayment(25, secondInstallment.Id);
            Assert.Equal(50, paymentPlan.OustandingBalance());
        }

        [Fact]
        public void WhenAttemptingASecondRefundShouldThrow()
        {
            var paymentPlan = new PaymentPlan(100, 4);

            var firstInstallment = paymentPlan.FirstInstallment();
            paymentPlan.MakePayment(25, firstInstallment.Id);
            Refund refund = new Refund(Guid.NewGuid().ToString(), 100);
            paymentPlan.ApplyRefund(refund);

            Assert.Throws<Exception>(() => {
                paymentPlan.ApplyRefund(refund);
            });
        }

        [Fact]
        public void ShouldRefundNothingIfNothingPaid(){
            var paymentPlan = new PaymentPlan(100, 4);
            Refund refund = new Refund(Guid.NewGuid().ToString(), 100);
            var cashRefund = paymentPlan.ApplyRefund(refund);
            Assert.Equal(0, cashRefund);
        }

        [Fact]
        public void OutstandingBalanceShouldReflectDefaultedPayments(){
            var paymentPlan = new PaymentPlan(100, 4);
            var firstInstallment = paymentPlan.FirstInstallment();
            paymentPlan.MakePayment(25, firstInstallment.Id);
            firstInstallment.SetDefaulted();
            Assert.Equal(100, paymentPlan.OustandingBalance());
        }

        [Fact]
        public void WhenPaymentPastDueAmountPastDueReflectsPayment(){
            var paymentPlan = new PaymentPlan(100, 4);
            Assert.Equal(25, paymentPlan.AmountPastDue());
        }

        [Fact]
        public void RefundShouldNotReflectDefaultedPayments(){
            var paymentPlan = new PaymentPlan(100, 4);
            var firstInstallment = paymentPlan.FirstInstallment();
            paymentPlan.MakePayment(25, firstInstallment.Id);
            firstInstallment.SetDefaulted();
            Refund refund = new Refund(Guid.NewGuid().ToString(), 100);
            var cashRefund = paymentPlan.ApplyRefund(refund);
            Assert.Equal(0, cashRefund);
        }

        [Theory]
        [InlineData(20)]
        [InlineData(-5)]
        [InlineData(60)]
        public void PaymentAmountIncorrectShouldThrowArgumentException(int paymentAmount)
        {
            var paymentPlan = new PaymentPlan(100, 4);
            var firstInstallment = paymentPlan.FirstInstallment();
            Assert.Throws<ArgumentException>(() =>
            {
                paymentPlan.MakePayment(paymentAmount, firstInstallment.Id);
            });
        }
    }
}
