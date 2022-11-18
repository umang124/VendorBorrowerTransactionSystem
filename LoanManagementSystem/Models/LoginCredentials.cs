using System;

namespace LoanManagementSystem.Models
{
    public class LoginCredentials
    {
        public Guid Id { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; } 
        public virtual User User { get; set; }
        public Guid UserId { get; set; }
    }
}