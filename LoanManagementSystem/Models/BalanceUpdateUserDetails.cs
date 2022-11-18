using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoanManagementSystem.Models
{
    public class BalanceUpdateUserDetails
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? ProceededAt { get; set; }
        public DateTime? ActionDate { get; set; }
        public string Action { get; set; }
        public int Status { get; set; }
    }
}