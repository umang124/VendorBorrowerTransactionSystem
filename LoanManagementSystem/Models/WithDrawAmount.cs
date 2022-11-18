using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoanManagementSystem.Models
{
    public class WithDrawAmount
    {
        //amount, u_pinCode, fromNumber
        public decimal Amount { get; set; }
        public string PinCode { get; set; }
        public string FromNumber { get; set; }
    }
}