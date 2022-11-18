using LoanManagementSystem.Constants;
using LoanManagementSystem.Data;
using LoanManagementSystem.DataAccessLayer;
using LoanManagementSystem.DTOs;
using LoanManagementSystem.ExceptionCustom;
using LoanManagementSystem.GetModel;
using LoanManagementSystem.Interface;
using LoanManagementSystem.Models;
using LoanManagementSystem.Repositories.RepositoryInterface;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using Telerivet.Client;

namespace LoanManagementSystem.Services
{
    public class UserService : IUserService
    {
        IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
        public ICollection<GetUser> GetBankers()
        {
            return _userRepository.GetBankers();
        }
        public ICollection<GetUser> GetUsers()
        {           
            return _userRepository.GetUsers();
        }
        public void AddBanker(RegisterDTO register)
        {
            User addUser = new User();
            addUser.Id = Guid.NewGuid();
            addUser.Name = register.Name;
            addUser.UserTypeId = 3;
            _userRepository.AddUsers(addUser);
            

            Guid userId = addUser.Id;

            CreatePasswordHash(register.Password, out byte[] passwordHash, out byte[] passwordSalt);

            LoginCredentials addLoginCredentials = new LoginCredentials();
            addLoginCredentials.Id = Guid.NewGuid();
            addLoginCredentials.PasswordHash = passwordHash;
            addLoginCredentials.PasswordSalt = passwordSalt; ;
            addLoginCredentials.UserId = userId;
            _userRepository.AddLoginCredentials(addLoginCredentials);

            AccountCredentials accountCredentials = new AccountCredentials();
            accountCredentials.Id = Guid.NewGuid();
            accountCredentials.UserId = userId;
            accountCredentials.PhoneNumber = register.PhoneNumber;
            _userRepository.AddAccountCredentials(accountCredentials);
        }
        public void AddAdmin(RegisterDTO register)
        {
            User addUser = new User();
            addUser.Id = Guid.NewGuid();
            addUser.Name = register.Name;
            addUser.UserTypeId = 4;
            _userRepository.AddUsers(addUser);

            Guid userId = addUser.Id;

            CreatePasswordHash(register.Password, out byte[] passwordHash, out byte[] passwordSalt);

            LoginCredentials addLoginCredentials = new LoginCredentials();
            addLoginCredentials.Id = Guid.NewGuid();
            addLoginCredentials.PasswordHash = passwordHash;
            addLoginCredentials.PasswordSalt = passwordSalt;
            addLoginCredentials.UserId = userId;
            _userRepository.AddLoginCredentials(addLoginCredentials);

            AccountCredentials accountCredentials = new AccountCredentials();
            accountCredentials.Id = Guid.NewGuid();
            accountCredentials.UserId = userId;
            accountCredentials.PhoneNumber = register.PhoneNumber;
            _userRepository.AddAccountCredentials(accountCredentials);
        }

        public AccountCredentials GetAccountCredential(Guid id)
        {          
            return _userRepository.GetAccountCredentialById(id);
        }
        public AccountCredentials GetAccountCredentialByNumber(string number)
        {
            return _userRepository.GetAccountCredentialByNumber(number);
        }
        public LoginCredentials GetLoginCredentials(Guid userId)
        {
            return _userRepository.GetLoginCredentials(userId);
        }
        public User GetUser(Guid userId)
        {
            return _userRepository.GetUser(userId);
        }
        public bool CheckAccountCredentialByNUmber(string PhoneNumber)
        {
            return _userRepository.CheckAccountCredentialByNUmber(PhoneNumber);
        }

        public void UpdateBalance(Guid id, UpdateUserBalanceDTO updateUserBalance)
        {
            _userRepository.UpdateAccountCredentials(id, updateUserBalance);
        }

        public bool RegisterUser(string u_type, string u_name, string u_pinCode, string fromNumber)
        {
            RegisterVenderOrBorrower _register = new RegisterVenderOrBorrower();
            _register.Name = u_name;
            _register.Type = u_type;
            _register.Pincode = u_pinCode;
            _register.FromNumber = fromNumber;
            // check user type exists

            int UserTypeId = Convert.ToInt32(_register.Type);
            var checkUserTypeExists = _userRepository.CheckUserTypeExists(UserTypeId);
            if (!checkUserTypeExists)
            {
                // return "Invalid UserType!";
                throw new CustomException(ExceptionMessage.INVALID_USERTYPE);
            }

            // check user type
            if (UserTypeId == 3)
            {
                throw new CustomException(ExceptionMessage.CANNOT_REGISTERED_AS_BANKER);
            }

            else if (UserTypeId == 4)
            {
                throw new CustomException(ExceptionMessage.CANNOT_REGISTERED_AS_ADMIN);
            }

            // check user phonenumber exists
            var checkUserExists = _userRepository.CheckPhonenumberAlreadyExists(_register.FromNumber);
            if (checkUserExists)
            {
                throw new CustomException(ExceptionMessage.PHONENUMBER_REGISTERED);
            }

            User user = new User();
            user.Id = Guid.NewGuid();
            user.UserTypeId = Convert.ToInt32(_register.Type);
            user.Name = _register.Name;

            // check valid pincode 
            bool isNumeric = int.TryParse(_register.Pincode, out int n);

            if (isNumeric == false)
                throw new CustomException(ExceptionMessage.PINCODE_MUST_BE_A_NUMBER);

            if (_register.Pincode.Length != 4)
            {
                throw new CustomException(ExceptionMessage.PINCODE_4_DIGIT);
            }

            _userRepository.AddUser(user);

            Guid userId = user.Id;

            PincodeCredentials pincodeCredentials = new PincodeCredentials();
            pincodeCredentials.Id = Guid.NewGuid();
            pincodeCredentials.UserId = userId;
            pincodeCredentials.PinCode = Convert.ToInt32(_register.Pincode);
            _userRepository.AddPincodeCredentials(pincodeCredentials);

            AccountCredentials accountCredentials = new AccountCredentials();
            accountCredentials.Id = Guid.NewGuid();
            accountCredentials.UserId = userId;
            accountCredentials.PhoneNumber = _register.FromNumber;
            _userRepository.AddAccountCredentials(accountCredentials);

            return true;
        }

    }
}