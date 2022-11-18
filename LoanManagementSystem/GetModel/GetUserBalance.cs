using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoanManagementSystem.GetModel
{
    public class GetUserBalance
    {
        public Guid UserId { get; set; }
        public decimal? Balance { get; set; }
    }
}