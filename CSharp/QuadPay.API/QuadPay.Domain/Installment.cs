using System;

namespace QuadPay.Domain
{
    public class Installment
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        private InstallmentStatus InstallmentStatus;
        public Guid PaymentReference { get; set;  }
        // Date this Installment was marked 'Paid'
        public DateTime SettlementDate { get; set; }

        public Installment(DateTime dueDate, decimal amount, InstallmentStatus installmentStatus = InstallmentStatus.Pending) {
            Amount = amount;
            Date = dueDate;
            InstallmentStatus = installmentStatus;
            Id = Guid.NewGuid();
        }

        public bool IsPaid { 
            get {
                return InstallmentStatus == InstallmentStatus.Paid;
            }
        }

        public bool IsDefaulted { 
            get {
                return InstallmentStatus == InstallmentStatus.Defaulted;
            }
        }

        public bool IsPending { 
            get {
                return InstallmentStatus == InstallmentStatus.Pending;
            }
        }

        public void SetPaid(Guid paymentReference, DateTime datePaid) {
            InstallmentStatus = InstallmentStatus.Paid;
            SettlementDate = datePaid;
            PaymentReference = paymentReference; 
        }

        public void SetDefaulted()
        {
            InstallmentStatus = InstallmentStatus.Defaulted;
        }
    }

    public enum InstallmentStatus {
        Pending = 0, // Not yet paid
        Paid = 1, // Can be either paid with a charge, or covered by a refund
        Defaulted = 2 // Charge failed
    }
}