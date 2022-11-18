using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoanManagementSystem.GetModel
{
    public class GetTransaction
    {
        public Guid TransactionId { get; set; }
        public string SenderMobileNumber { get; set; }
        public string ReceiverMobileNumber { get; set; }
        public decimal Amount { get; set; }
        public string Name { get; set; }
        public int ApproveStatus { get; set; }
        public DateTime ProceededAt { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}