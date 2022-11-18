using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoanManagementSystem.Models
{
    public class RegisterVenderOrBorrower
    {

        public string Type { get; set; }
        public string Name { get; set; }
        public string Pincode { get; set; }
        public string FromNumber { get; set; }
    }
}