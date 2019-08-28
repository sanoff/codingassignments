using System;

namespace QuadPay.Domain {
    public class Refund {

        public Guid Id { get; }
        public string IdempotencyKey { get; }
        public DateTime Date { get; }
        public decimal Amount { get; }
        public decimal AmountRefunded { get; set; } = 0;

        public Refund(string idempotencyKey, decimal amount) {
            IdempotencyKey = idempotencyKey;
            Date = DateTime.Now;
            Id = Guid.NewGuid();
            Amount = amount;
        }

    }
}