using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoanManagementSystem.Models
{
    public class BalanceUpdateBankerDetails
    {
        public Guid Id { get; set; }
        public Guid ActionDoneBy { get; set; }
        public Guid BalanceUpdateUserDetailsId { get; set; }
    }
}