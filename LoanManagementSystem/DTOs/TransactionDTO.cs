using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoanManagementSystem.DTOs
{
    public class TransactionDTO
    {
        public Guid TransactionId { get; set; }
        public int ApproveStatus { get; set; }
    }
}