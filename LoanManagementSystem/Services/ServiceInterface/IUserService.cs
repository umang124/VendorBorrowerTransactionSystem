
using LoanManagementSystem.DTOs;
using LoanManagementSystem.GetModel;
using LoanManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanManagementSystem.Interface
{
    public interface IUserService
    {
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        ICollection<GetUser> GetBankers();
        ICollection<GetUser> GetUsers();
        void AddBanker(RegisterDTO register);
        void AddAdmin(RegisterDTO register);
        AccountCredentials GetAccountCredential(Guid id);
        AccountCredentials GetAccountCredentialByNumber(string number);
        LoginCredentials GetLoginCredentials(Guid userId);
        User GetUser(Guid userId);
        bool CheckAccountCredentialByNUmber(string PhoneNumber);
        void UpdateBalance(Guid id, UpdateUserBalanceDTO updateUserBalance);
        bool RegisterUser(string u_type, string u_name, string u_pinCode, string fromNumber);
    }
}
