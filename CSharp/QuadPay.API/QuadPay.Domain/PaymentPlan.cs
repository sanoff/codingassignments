using System;
using System.Collections.Generic;
using System.Linq;

namespace QuadPay.Domain
{
    public class PaymentPlan
    {
        public Guid Id { get; }
        public IList<Installment> Installments { get; }
        public IList<Refund> Refunds { get; }
        public DateTime OriginationDate { get; }
        private decimal InstallmentAmount { get; }
        private int InstallmentIntervalDays { get; }
        private int InstallmentCount { get; }

        public PaymentPlan(decimal amount, int installmentCount = 4, int installmentIntervalDays = 14) {
            if (amount <= 0 || installmentCount < 1 || installmentIntervalDays < 1)
            {
                // Could be separated out into different exceptions for each type of parameter issue.
                throw new ArgumentException();
            }

            InstallmentAmount = Math.Round(amount / installmentCount, 2);
            InstallmentCount = installmentCount;
            InstallmentIntervalDays = installmentIntervalDays;
            Installments = new List<Installment>();
            Refunds = new List<Refund>();
            OriginationDate = DateTime.Now;
            InitializeInstallments();
        }

        // Installments are paid in order by Date
        public Installment NextInstallment() {
            return Installments.FirstOrDefault(i => !i.IsPaid);
        }

        public Installment FirstInstallment() {
            return Installments.First();
        }

        public decimal OustandingBalance() {
            return PendingInstallments().Sum(i => i.Amount) + DefaultedInstallments().Sum(i => i.Amount);
        }

        public decimal AmountPastDue() {
            DateTime currentDate = DateTime.Now;
            return Installments.Where(i => (i.IsDefaulted || i.IsPending) && currentDate > i.Date).Sum(i => i.Amount);
        }

        public IList<Installment> PaidInstallments() {  
            return Installments.Where(i => i.IsPaid).ToList();
        }

        public IList<Installment> DefaultedInstallments() {
            
            return Installments.Where(i => i.IsDefaulted).ToList();
        }

        public IList<Installment> PendingInstallments() {
            return Installments.Where(i => i.IsPending).ToList();
        }

        public decimal MaximumRefundAvailable() {
            return PaidInstallments().Sum(i => i.Amount);
        }

        // We only accept payments matching the Installment Amount.
        public void MakePayment(decimal amount, Guid installmentId) {
            Installment installment = Installments.First(i => i.Id == installmentId);
            if (amount != installment.Amount)
            {
                throw new ArgumentException();
            }
            installment.SetPaid(Id, DateTime.Now);
        }

        // Returns: Amount to refund via PaymentProvider
        public decimal ApplyRefund(Refund refund) {
            // If we haven't already tried to apply this refund
            if (Refunds.Select(r => r.IdempotencyKey).Contains(refund.IdempotencyKey))
            {
                throw new Exception();
            }

            refund.AmountRefunded = MaximumRefundAvailable();
            Refunds.Add(refund);

            foreach (Installment installment in Installments)
            {
                // Do this so everything gets set as 'paid'
                installment.SetPaid(Id, DateTime.Now);
            }
            return refund.AmountRefunded;
        }

        // First Installment always occurs on PaymentPlan creation date
        private void InitializeInstallments() {
            for (int interval = 0; interval < InstallmentCount; interval++)
            {
                Installments.Add(new Installment(OriginationDate.AddDays(interval * InstallmentIntervalDays), InstallmentAmount));
            }
            Installments.OrderBy(i => i.Date);
        }
    }
}