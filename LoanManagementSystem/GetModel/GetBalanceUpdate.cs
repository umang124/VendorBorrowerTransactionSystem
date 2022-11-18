using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoanManagementSystem.GetModel
{
    public class GetBalanceUpdate
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public DateTime? ProceededAt { get; set; }
        public DateTime? ActionDate { get; set; }
        public string Action { get; set; }
        public int Status { get; set; }
        public Guid ActionDoneBy { get; set; }
    }
}