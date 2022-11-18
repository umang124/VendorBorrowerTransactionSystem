using LoanManagementSystem.Data;
using LoanManagementSystem.DTOs;
using LoanManagementSystem.GetModel;
using LoanManagementSystem.Models;
using LoanManagementSystem.Repositories.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoanManagementSystem.DataAccessLayer
{
    public class UserRepository : IUserRepository
    {
        LMSDbContext _dbContext = new LMSDbContext();
        public ICollection<GetUser> GetBankers()
        {
            var getBankers = _dbContext.Database.SqlQuery<GetUser>(@"select u.[Id] as Id, u.[Name] as UserName, ut.[Name] as UserTypeName, 
                        a.[PhoneNumber] as PhoneNumber, a.[Balance] as Balance
                        from [LoanManagementSystem].[dbo].[User] as u inner join [LoanManagementSystem].[dbo].[UserType] as ut 
                        on ut.[Id] = u.[UserTypeId] inner join [LoanManagementSystem].[dbo].[AccountCredentials] as a 
                        on a.[UserId] = u.[Id] where ut.[Id] = 3").ToList();
            return getBankers;
        }
        public ICollection<GetUser> GetUsers()
        {
            var getUsers = _dbContext.Database.SqlQuery<GetUser>(@"select u.[Id] as Id, u.[Name] as UserName, ut.[Name] as UserTypeName,
                a.[PhoneNumber] as PhoneNumber, a.[Balance] as Balance,
                p.[PinCode] as PinCode from [LoanManagementSystem].[dbo].[User] as u
                inner join [LoanManagementSystem].[dbo].[UserType] as ut
                on ut.[Id] = u.[UserTypeId]
                inner join [LoanManagementSystem].[dbo].[AccountCredentials] as a
                on a.[UserId] = u.[Id]
                inner join [LoanManagementSystem].[dbo].[PincodeCredentials] as p
                on p.[UserId] = u.[Id]
                where ut.[Id] != 3 and ut.[Id] != 4").ToList();
            return getUsers;
        }
        public User GetUser(Guid userId)
        {
            return _dbContext.Users.FirstOrDefault(x => x.Id == userId);
        }
        public bool CheckAccountCredentialByNUmber(string PhoneNumber)
        {
            return _dbContext.AccountCredentials.Any(u => u.PhoneNumber == PhoneNumber);
        }

        public AccountCredentials GetAccountCredentialByNumber(string number)
        {
            var getBalance = _dbContext.AccountCredentials.FirstOrDefault(x => x.PhoneNumber == number);
            return getBalance;
        }
        public LoginCredentials GetLoginCredentials(Guid userId)
        {
            return _dbContext.LoginCredentials.FirstOrDefault(x => x.UserId == userId);
        }
        public void AddUsers(User addUser)
        {
            _dbContext.Users.Add(addUser);
            _dbContext.SaveChanges();
        }
        public void AddLoginCredentials(LoginCredentials addLoginCredentials)
        {
            _dbContext.LoginCredentials.Add(addLoginCredentials);
            _dbContext.SaveChanges();
        }
        public void AddAccountCredentials(AccountCredentials accountCredentials)
        {
            _dbContext.AccountCredentials.Add(accountCredentials);
            _dbContext.SaveChanges();
        }
        public AccountCredentials GetAccountCredentialById(Guid id)
        {
            return _dbContext.AccountCredentials.FirstOrDefault(x => x.UserId == id);
        }

        public void UpdateAccountCredentials(Guid id, UpdateUserBalanceDTO updateUserBalance)
        {
            var getUser = GetAccountCredentialById(id);
            getUser.Balance = updateUserBalance.Balance;
            _dbContext.SaveChanges();
        }
        public AccountCredentials GetAccountCredentials(string SenderMobileNumber)
        {
            return _dbContext.AccountCredentials.FirstOrDefault(x => x.PhoneNumber == SenderMobileNumber);
        }
        public bool CheckUserTypeExists(int UserTypeId)
        {
            return _dbContext.UserTypes.Any(x => x.Id == UserTypeId);
        }
        public bool CheckPhonenumberAlreadyExists(string fromNumber)
        {
            return _dbContext.AccountCredentials.Any(x => x.PhoneNumber == fromNumber);
        }
        public bool CheckPhoneNumberNotRegistered(string fromNumber)
        {
            return _dbContext.AccountCredentials.Any(x => x.PhoneNumber == fromNumber);
        }

        public AccountCredentials GetBalanceToCheck(string fromNumber)
        {
            return _dbContext.AccountCredentials.FirstOrDefault(x => x.PhoneNumber == fromNumber); ;
        }
        public bool CheckPincode(Guid UserId, int PinCode)
        {
            return _dbContext.PincodeCredentials.Any(x => (x.UserId == UserId) && (x.PinCode == PinCode));
        }
        public void AddUser(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }
        public void AddPincodeCredentials(PincodeCredentials pincodeCredentials)
        {
            _dbContext.PincodeCredentials.Add(pincodeCredentials);
            _dbContext.SaveChanges();
        }

        public bool ValidateTransactionByUserType(string fromNumber)
        {
            var getAccountCredentials = _dbContext.AccountCredentials.FirstOrDefault(x => x.PhoneNumber == fromNumber);
            Guid userId = getAccountCredentials.UserId;

            var getUser = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
            var getUserType = getUser.UserTypeId;
            if (getUserType != 1)
            {
                return false;
            }
            return true;
        }

    }
}