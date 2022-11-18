using System;
using System.ComponentModel.DataAnnotations;

namespace LoanManagementSystem.DTOs
{
    public class RegisterDTO
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }

        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}