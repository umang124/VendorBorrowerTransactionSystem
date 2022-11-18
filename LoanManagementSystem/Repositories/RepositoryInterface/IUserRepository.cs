using LoanManagementSystem.DTOs;
using LoanManagementSystem.GetModel;
using LoanManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace LoanManagementSystem.Repositories.RepositoryInterface
{
    public interface IUserRepository
    {
        ICollection<GetUser> GetBankers();
        ICollection<GetUser> GetUsers();
        User GetUser(Guid userId);
        bool CheckAccountCredentialByNUmber(string PhoneNumber);
        AccountCredentials GetAccountCredentialByNumber(string number);
        LoginCredentials GetLoginCredentials(Guid userId);
        void AddUsers(User addUser);
        void AddLoginCredentials(LoginCredentials addLoginCredentials);
        void AddAccountCredentials(AccountCredentials accountCredentials);
        AccountCredentials GetAccountCredentialById(Guid id);
        void UpdateAccountCredentials(Guid id, UpdateUserBalanceDTO updateUserBalance);
        AccountCredentials GetAccountCredentials(string SenderMobileNumber);
        bool CheckUserTypeExists(int UserTypeId);
        bool CheckPhonenumberAlreadyExists(string fromNumber);
        bool CheckPhoneNumberNotRegistered(string fromNumber);
        AccountCredentials GetBalanceToCheck(string fromNumber);
        bool CheckPincode(Guid UserId, int PinCode);
        void AddUser(User user);
        void AddPincodeCredentials(PincodeCredentials pincodeCredentials);
        bool ValidateTransactionByUserType(string fromNumber);
    }
}
