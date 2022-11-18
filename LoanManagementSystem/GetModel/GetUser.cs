using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoanManagementSystem.GetModel
{
    public class GetUser
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string UserTypeName { get; set; }
        public string PhoneNumber { get; set; }
        public decimal? Balance { get; set; }
        public int? PinCode { get; set; }
    }
}