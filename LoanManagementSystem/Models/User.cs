using System;
using System.Collections.Generic;

namespace LoanManagementSystem.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual UserType UserType { get; set; }
        public int UserTypeId { get; set; }
    }
}