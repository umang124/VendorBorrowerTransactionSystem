using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoanManagementSystem.Models
{
    public class CreateTransaction
    {
        // toNumber, amount, u_pinCode, fromNumber
        public string ToNumber { get; set; }
        public decimal Amount { get; set; }
        public string Pincode { get; set; }
        public string FromNumber { get; set; }
    }
}