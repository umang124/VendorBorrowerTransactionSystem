using System;

namespace LoanManagementSystem.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public string SenderMobileNumber { get; set; }
        public string ReceiverMobileNumber { get; set; }
        public decimal Amount { get; set; }
        public int ApproveStatus { get; set; }
        public virtual User User { get; set; }
        public Guid? UserId { get; set; }
        public DateTime ProceededAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}