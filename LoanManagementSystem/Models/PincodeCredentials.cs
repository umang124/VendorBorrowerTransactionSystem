using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoanManagementSystem.Models
{
    public class PincodeCredentials
    {
        public Guid Id { get; set; }
        public int PinCode { get; set; }
        public virtual User User { get; set; }
        public Guid UserId { get; set; }
    }
}