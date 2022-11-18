using LoanManagementSystem.Constants;
using LoanManagementSystem.DataAccessLayer;
using LoanManagementSystem.ExceptionCustom;
using LoanManagementSystem.GetModel;
using LoanManagementSystem.Interface;
using LoanManagementSystem.Models;
using LoanManagementSystem.Repositories.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoanManagementSystem.Services
{
    public class TransactionService : ITransactionService
    {
        ITransactionRepository _transactionRepository;
        IUserRepository _userRepository;

        public TransactionService(ITransactionRepository transactionRepository, IUserRepository userRepository)
        {
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
        }

        public ICollection<GetTransaction> GetUnApprovedTransactions()
        {
            var getTransactions = _transactionRepository.GetUnApprovedTransactions();
            return getTransactions;
        }
        public ICollection<GetTransaction> GetApprovedTransactions()
        {
            var getTransactions = _transactionRepository.GetApprovedTransactions();
            return getTransactions;
        }

        public ICollection<GetTransaction> GetRejectedTransactions()
        {
            var getTransactions = _transactionRepository.GetRejectedTransactions();
            return getTransactions;
        }

        public async Task<bool> ApproveStatus(Guid transactionId, string bankerPhoneNumber)
        {
            return await _transactionRepository.ApproveStatusDataManager(transactionId, bankerPhoneNumber);
        }
        public async Task<bool> RejectStatus(Guid transactionId, string bankerPhoneNumber)
        {
            return await _transactionRepository.RejectStatusDataManager(transactionId, bankerPhoneNumber);
        }
        public ICollection<GetBalanceUpdate> GetUnApprovedWithdraw()
        {
            return _transactionRepository.GetUnApprovedWithdraw();
        }
        public ICollection<GetBalanceUpdate> GetApprovedWithdraw()
        {
            return _transactionRepository.GetApprovedWithdraw();
        }
        public ICollection<GetBalanceUpdate> GetRejectedWithdraw()
        {
            return _transactionRepository.GetRejectedWithdraw();
        }
        public ICollection<GetBalanceUpdate> GetDetailsOfBalanceUpdatedByBanker()
        {
            return _transactionRepository.GetDetailsOfBalanceUpdatedByBanker();
        }
        public async Task RejectWithdraw(string getBankerMobileNumber, Guid b_id)
        {
            await _transactionRepository.RejectWithdrawManager(getBankerMobileNumber, b_id);
        }
        public async Task ApproveWithdraw(string getBankerMobileNumber, Guid b_id)
        {
            await _transactionRepository.ApproveWithdrawManager(getBankerMobileNumber, b_id);
        }
        public void InsertBalanceDetails(Guid id, decimal amountAdded, Guid BankerUserId)
        {
            _transactionRepository.InsertBalanceDetails(id, amountAdded, BankerUserId);
        }
        public bool WithDrawAmount(decimal amount, string u_pinCode, string fromNumber)
        {
            WithDrawAmount _withDrawAmount = new WithDrawAmount();
            _withDrawAmount.Amount = amount;
            _withDrawAmount.PinCode = u_pinCode;
            _withDrawAmount.FromNumber = fromNumber;
            if (!_userRepository.CheckPhonenumberAlreadyExists(_withDrawAmount.FromNumber))
            {
                throw new CustomException(ExceptionMessage.PHONENUMBER_NOT_REGISTERED);
            }

            bool isNumeric1 = int.TryParse(_withDrawAmount.PinCode, out int result);

            if (!isNumeric1)
            {
                throw new CustomException(ExceptionMessage.PINCODE_MUST_BE_A_NUMBER);
            }

            AccountCredentials getAccount = _userRepository.GetAccountCredentials(_withDrawAmount.FromNumber);

            Guid UserId = getAccount.UserId;

            int PinCode = Convert.ToInt32(_withDrawAmount.PinCode);

            // check pincode
            if (!_userRepository.CheckPincode(UserId, PinCode))
            {
                throw new CustomException(ExceptionMessage.INVALID_PINCODE);
            }
            // check balance
        //    AccountCredentials checkBalance = _userRepository.GetAccountCredentials(_withDrawAmount.FromNumber);
            var getBalance = getAccount.Balance;

            if (getBalance < _withDrawAmount.Amount)
            {

                throw new CustomException(ExceptionMessage.NOT_ENOUGH_BALANCE);
            }

            // UserId, Amount, ProceededAt, Action, Status
          //  AccountCredentials getUser = _userRepository.GetAccountCredentials(_withDrawAmount.FromNumber);

            BalanceUpdateUserDetails balanceUpdate = new BalanceUpdateUserDetails();
            balanceUpdate.Id = Guid.NewGuid();
            balanceUpdate.UserId = getAccount.UserId;
            balanceUpdate.Amount = _withDrawAmount.Amount;
            balanceUpdate.ProceededAt = DateTime.UtcNow;
            balanceUpdate.Action = "Action Amount WithDraw";
            balanceUpdate.Status = 0;

            _transactionRepository.BalanceUpdateUserDetails(balanceUpdate);

            return true;

        }
        public bool CreateTransaction(string toNumber, decimal amount, string u_pinCode, string fromNumber)
        {
            CreateTransaction _createTransaction = new CreateTransaction();
            _createTransaction.ToNumber = toNumber;
            _createTransaction.Amount = amount;
            _createTransaction.Pincode = u_pinCode;
            _createTransaction.FromNumber = fromNumber;

            if (_createTransaction.FromNumber == _createTransaction.ToNumber)
            {
                throw new CustomException(ExceptionMessage.INVALID_PHONENUMBER);
            }
            // check sender number
            if (!_userRepository.CheckPhonenumberAlreadyExists(_createTransaction.FromNumber))
            {
                throw new CustomException(ExceptionMessage.PHONENUMBER_NOT_REGISTERED);
            }
            AccountCredentials getAccount = _userRepository.GetAccountCredentials(_createTransaction.FromNumber);

            Guid UserId = getAccount.UserId;

            int PinCode = Convert.ToInt32(_createTransaction.Pincode);
            // check pincode
            if (!_userRepository.CheckPincode(UserId, PinCode))
            {
                throw new CustomException(ExceptionMessage.INVALID_PINCODE);
            }
            // check receiver number
            if (!_userRepository.CheckPhonenumberAlreadyExists(_createTransaction.FromNumber))
            {
                throw new CustomException(ExceptionMessage.RECEIVER_NUMBER_NOT_EXIST);
            }
            if (!_userRepository.ValidateTransactionByUserType(fromNumber))
            {
                throw new CustomException(ExceptionMessage.VENDOR_CANNOT_CREATE_TRANSACTION);
            }

            Transaction transaction = new Transaction();
            transaction.Id = Guid.NewGuid();
            transaction.SenderMobileNumber = _createTransaction.FromNumber;
            transaction.ReceiverMobileNumber = _createTransaction.ToNumber;

            transaction.Amount = _createTransaction.Amount;
            transaction.ApproveStatus = 0;
            transaction.ProceededAt = DateTime.UtcNow;

            _transactionRepository.AddTransaction(transaction);
            return true;
        }

    }
}