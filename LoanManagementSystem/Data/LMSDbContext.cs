using LoanManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace LoanManagementSystem.Data
{
    public class LMSDbContext : DbContext
    {
        public LMSDbContext() : base("LMSDbContext")
        {

        }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<LoginCredentials> LoginCredentials { get; set; }
        public DbSet<PincodeCredentials> PincodeCredentials { get; set; }
        public DbSet<AccountCredentials> AccountCredentials { get; set; }
        public DbSet<BalanceUpdateBankerDetails> BalanceUpdateBankerDetails { get; set; }
        public DbSet<BalanceUpdateUserDetails> BalanceUpdateUserDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}