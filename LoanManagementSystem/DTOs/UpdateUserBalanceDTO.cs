using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoanManagementSystem.DTOs
{
    public class UpdateUserBalanceDTO
    {
        public Guid Id { get; set; }
        [Required]
        public decimal Balance { get; set; }
    }
}