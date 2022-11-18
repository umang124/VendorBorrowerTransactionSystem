using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoanManagementSystem.Models
{
    public class AccountCredentials
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Balance { get; set; }
        public virtual User User { get; set; }
        public Guid UserId { get; set; }
    }
}