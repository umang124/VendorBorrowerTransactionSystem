using LoanManagementSystem.Constants;
using LoanManagementSystem.Data;
using LoanManagementSystem.DataAccessLayer;
using LoanManagementSystem.ExceptionCustom;
using LoanManagementSystem.GetModel;
using LoanManagementSystem.Models;
using LoanManagementSystem.Repositories.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.EnterpriseServices;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Util;
using Telerivet.Client;

namespace LoanManagementSystem
{
    public class TransactionRepository : ITransactionRepository
    {
        readonly LMSDbContext _dbContext = new LMSDbContext();
        readonly UserRepository _userRepository = new UserRepository();
        readonly TelerivetAPI tr;
        readonly Project project;
        public TransactionRepository()
        {
            tr = new TelerivetAPI(WebConfigurationManager.AppSettings["API_Key"]);
            project = tr.InitProjectById(WebConfigurationManager.AppSettings["ProjectId"]);
           
        }

        public ICollection<GetTransaction> GetUnApprovedTransactions()
        {
            return _dbContext.Transactions.Include(x => x.User)
                                    .Select(s => new GetTransaction
                                    {
                                        TransactionId = s.Id,
                                        SenderMobileNumber = s.SenderMobileNumber,
                                        ReceiverMobileNumber = s.ReceiverMobileNumber,
                                        Amount = s.Amount,
                                        Name = s.User.Name,
                                        ApproveStatus = s.ApproveStatus,
                                        ProceededAt = s.ProceededAt
                                    }).Where(x => x.ApproveStatus == 0).ToList();
        }
        public ICollection<GetTransaction> GetApprovedTransactions()
        {
            var getTransactions = _dbContext.Transactions.Include(x => x.User)
                                    .Select(s => new GetTransaction
                                    {
                                        TransactionId = s.Id,
                                        SenderMobileNumber = s.SenderMobileNumber,
                                        ReceiverMobileNumber = s.ReceiverMobileNumber,
                                        Amount = s.Amount,
                                        Name = s.User.Name,
                                        ApproveStatus = s.ApproveStatus,
                                        ProceededAt = s.ProceededAt
                                    }).Where(x => x.ApproveStatus == 1).ToList();
            return getTransactions;
        }
        public ICollection<GetTransaction> GetRejectedTransactions()
        {
            var getTransactions = _dbContext.Transactions.Include(x => x.User)
                                    .Select(s => new GetTransaction
                                    {
                                        TransactionId = s.Id,
                                        SenderMobileNumber = s.SenderMobileNumber,
                                        ReceiverMobileNumber = s.ReceiverMobileNumber,
                                        Amount = s.Amount,
                                        Name = s.User.Name,
                                        ApproveStatus = s.ApproveStatus,
                                        ProceededAt = s.ProceededAt
                                    }).Where(x => x.ApproveStatus == 3).ToList();
            return getTransactions;
        }
        public Transaction GetTransaction(Guid transactionId)
        {
            return _dbContext.Transactions.FirstOrDefault(x => x.Id == transactionId); ;
        }

        public ICollection<GetBalanceUpdate> GetUnApprovedWithdraw()
        {
            var getUnApprovedWithdraw = _dbContext.Database.SqlQuery<GetBalanceUpdate>(@"select u.Name as UserName,bu.Id, bu.Amount, 
                bu.ProceededAt, bu.ActionDate, bu.Action, bu.Status from BalanceUpdateUserDetails as bu
                inner join [User] as u on u.[Id] = bu.[UserId]
                where bu.Status = 0").ToList();
            return getUnApprovedWithdraw;
        }
        public ICollection<GetBalanceUpdate> GetApprovedWithdraw()
        {
            var getApprovedWithdraw = _dbContext.Database.SqlQuery<GetBalanceUpdate>(@"select bu.Id, u.Name as UserName, bu.Amount, 
                    bu.ProceededAt, bu.ActionDate, bu.Action, bu.Status, ba.ActionDoneBy as ActionDoneBy from BalanceUpdateUserDetails as bu
                    inner join [User] as u on u.[Id] = bu.[UserId]
                    inner join [BalanceUpdateBankerDetails] as ba
                    on ba.[BalanceUpdateUserDetailsId] = bu.[Id]
                    where bu.Status = 1").ToList();
            return getApprovedWithdraw;
        }
        public ICollection<GetBalanceUpdate> GetRejectedWithdraw()
        {
            var getRejectedWithdraw = _dbContext.Database.SqlQuery<GetBalanceUpdate>(@"select bu.Id, u.Name as UserName, bu.Amount, 
                bu.ProceededAt, bu.ActionDate, bu.Action, bu.Status, ba.ActionDoneBy as ActionDoneBy from BalanceUpdateUserDetails as bu
                inner join [User] as u on u.[Id] = bu.[UserId]
                inner join [BalanceUpdateBankerDetails] as ba
                on ba.[BalanceUpdateUserDetailsId] = bu.[Id]
                where bu.Status = 2").ToList();
            return getRejectedWithdraw;
        }
        public ICollection<GetBalanceUpdate> GetDetailsOfBalanceUpdatedByBanker()
        {
            var getDetails = _dbContext.Database.SqlQuery<GetBalanceUpdate>(@"select bu.Id, u.Name as UserName, bu.Amount, 
                bu.ProceededAt, bu.ActionDate, bu.Action, bu.Status, ba.ActionDoneBy as ActionDoneBy from BalanceUpdateUserDetails as bu
                inner join [User] as u on u.[Id] = bu.[UserId]
                inner join [BalanceUpdateBankerDetails] as ba
                on ba.[BalanceUpdateUserDetailsId] = bu.[Id]
                where bu.Status = 3").ToList();
            return getDetails;
        }
        public async Task<bool> RejectWithdrawManager(string getBankerMobileNumber, Guid b_id)
        {
            int status = 2;

            if (!_dbContext.AccountCredentials.Any(x => x.PhoneNumber == getBankerMobileNumber))
            {
                throw new CustomException(ExceptionMessage.PHONENUMBER_NOT_REGISTERED);
            }
            if (!_dbContext.BalanceUpdateUserDetails.Any(x => x.Id == b_id))
            {
                throw new CustomException(ExceptionMessage.INVALID_ID);
            }

            var getAcc = _dbContext.AccountCredentials.FirstOrDefault(x => x.PhoneNumber == getBankerMobileNumber);

            Guid bankerId = getAcc.UserId;


            BalanceUpdateBankerDetails balanceUpdateBankerDetails = new BalanceUpdateBankerDetails();
            balanceUpdateBankerDetails.Id = Guid.NewGuid();
            balanceUpdateBankerDetails.ActionDoneBy = bankerId;
            balanceUpdateBankerDetails.BalanceUpdateUserDetailsId = b_id;
            _dbContext.BalanceUpdateBankerDetails.Add(balanceUpdateBankerDetails);

            var balanceUpdateUserDetails = await _dbContext.BalanceUpdateUserDetails.FirstOrDefaultAsync(x => x.Id == b_id);
            balanceUpdateUserDetails.ActionDate = DateTime.UtcNow;
            balanceUpdateUserDetails.Status = status;
            _dbContext.SaveChanges();

            var updateUserBalance = _dbContext.AccountCredentials.FirstOrDefault(x => x.UserId == balanceUpdateUserDetails.UserId);

            await project.SendMessageAsync(Util.Options(
                                       "content", "Withdrawn rejected!",
                                       "to_number", updateUserBalance.PhoneNumber
                                   ));
            return true;
        }
        public async Task<bool> ApproveWithdrawManager(string getBankerMobileNumber, Guid b_id)
        {
            int Status = 1;

            if (!_dbContext.AccountCredentials.Any(x => x.PhoneNumber == getBankerMobileNumber))
            {
                throw new CustomException(ExceptionMessage.PHONENUMBER_NOT_REGISTERED);
            }
            if (!_dbContext.BalanceUpdateUserDetails.Any(x => x.Id == b_id))
            {
                throw new CustomException(ExceptionMessage.INVALID_ID);
            }

            var getAcc = _dbContext.AccountCredentials.FirstOrDefault(x => x.PhoneNumber == getBankerMobileNumber);

            Guid bankerId = getAcc.UserId;


            BalanceUpdateBankerDetails balanceUpdateBankerDetails = new BalanceUpdateBankerDetails();
            balanceUpdateBankerDetails.Id = Guid.NewGuid();
            balanceUpdateBankerDetails.ActionDoneBy = bankerId;
            balanceUpdateBankerDetails.BalanceUpdateUserDetailsId = b_id;
            _dbContext.BalanceUpdateBankerDetails.Add(balanceUpdateBankerDetails);

            var balanceUpdateUserrDetails = _dbContext.BalanceUpdateUserDetails.FirstOrDefault(x => x.Id == b_id);
            balanceUpdateUserrDetails.ActionDate = DateTime.UtcNow;
            balanceUpdateUserrDetails.Status = Status;

            var updateUserBalance = _dbContext.AccountCredentials.FirstOrDefault(x => x.UserId == balanceUpdateUserrDetails.UserId);
            var finalBalance = updateUserBalance.Balance - balanceUpdateUserrDetails.Amount;
            updateUserBalance.Balance = finalBalance;

            await project.SendMessageAsync(Util.Options(
                                       "content", balanceUpdateUserrDetails.Amount + " Amount has been withdrawn!",
                                       "to_number", updateUserBalance.PhoneNumber
                                   ));
            _dbContext.SaveChanges();
            return true;
        }
        public bool InsertBalanceDetails(Guid id, decimal amountAdded, Guid BankerUserId)
        {

            if (!_dbContext.Users.Any(x => x.Id == id))
            {
                throw new CustomException(ExceptionMessage.INVALID_ID);
            }
            if (!_dbContext.Users.Any(x => x.Id == BankerUserId))
            {
                throw new CustomException(ExceptionMessage.INVALID_ID);
            }

            BalanceUpdateUserDetails balanceUpdateUserDetails = new BalanceUpdateUserDetails();
            balanceUpdateUserDetails.Id = Guid.NewGuid();
            balanceUpdateUserDetails.UserId = id;
            balanceUpdateUserDetails.Amount = amountAdded;
            balanceUpdateUserDetails.ActionDate = DateTime.UtcNow;
            balanceUpdateUserDetails.Action = "Action Amount Insert";
            balanceUpdateUserDetails.Status = 3;
            _dbContext.BalanceUpdateUserDetails.Add(balanceUpdateUserDetails);
            _dbContext.SaveChanges();


            var BalanceUpdateUserDetailsId = balanceUpdateUserDetails.Id;

            BalanceUpdateBankerDetails balanceUpdateBankerDetails = new BalanceUpdateBankerDetails();
            balanceUpdateBankerDetails.Id = Guid.NewGuid();
            balanceUpdateBankerDetails.ActionDoneBy = BankerUserId;
            balanceUpdateBankerDetails.BalanceUpdateUserDetailsId = BalanceUpdateUserDetailsId;
            _dbContext.BalanceUpdateBankerDetails.Add(balanceUpdateBankerDetails);

            _dbContext.SaveChanges();
            return true;
        }

      
        public async Task<bool> ApproveStatusDataManager(Guid transactionId, string bankerPhoneNumber)
        {
            if (!_dbContext.AccountCredentials.Any(x => x.PhoneNumber == bankerPhoneNumber))
            {
                throw new CustomException(ExceptionMessage.PHONENUMBER_NOT_REGISTERED);
            }
            if (!_dbContext.Transactions.Any(x => x.Id == transactionId))
            {
                throw new CustomException(ExceptionMessage.INVALID_ID);
            }
            var getTransaction = GetTransaction(transactionId);
            int approveStatus = 1;
            getTransaction.ApproveStatus = approveStatus;
            getTransaction.CompletedAt = DateTime.UtcNow;

            var getSenderDetails = _userRepository.GetAccountCredentials(getTransaction.SenderMobileNumber);

            var updatedSenderAmount = getSenderDetails.Balance - getTransaction.Amount;
            getSenderDetails.Balance = updatedSenderAmount;


            var getReceiverDetails = _userRepository.GetAccountCredentials(getTransaction.ReceiverMobileNumber);
            var updatedReceiverAmount = getReceiverDetails.Balance + getTransaction.Amount;
            getReceiverDetails.Balance = updatedReceiverAmount; 

            //var bankerPhoneNumber = User.Identity.Name;
            var getBanker = _userRepository.GetAccountCredentials(bankerPhoneNumber);
            getTransaction.UserId = getBanker.UserId;
            _dbContext.SaveChanges();

            await project.SendMessageAsync(Util.Options(
                                       "content", "Transaction succeded! Amount " + getTransaction.Amount + " has been sent to " + getTransaction.ReceiverMobileNumber,
                                       "to_number", getTransaction.SenderMobileNumber
                                   ));

            await project.SendMessageAsync(Util.Options(
                                      "content", "You have received amount " + getTransaction.Amount + " from " + getTransaction.SenderMobileNumber,
                                      "to_number", getTransaction.ReceiverMobileNumber
                                  ));
            return true;
        }
        public async Task<bool> RejectStatusDataManager(Guid transactionId, string bankerPhoneNumber)
        {
            if (!_dbContext.AccountCredentials.Any(x => x.PhoneNumber == bankerPhoneNumber))
            {
                throw new CustomException(ExceptionMessage.PHONENUMBER_NOT_REGISTERED);
            }
            if (!_dbContext.Transactions.Any(x => x.Id == transactionId))
            {
                throw new CustomException(ExceptionMessage.INVALID_ID);
            }

            int approveStatus = 3;

            var getBanker = _dbContext.AccountCredentials.FirstOrDefault(x => x.PhoneNumber == bankerPhoneNumber);

            var getTransaction = _dbContext.Transactions.FirstOrDefault(x => x.Id == transactionId);
            getTransaction.ApproveStatus = approveStatus;
            getTransaction.UserId = getBanker.UserId;

            await project.SendMessageAsync(Util.Options(
                                       "content", "Transaction rejected!",
                                       "to_number", getTransaction.SenderMobileNumber
                                   ));

            _dbContext.SaveChanges();
            return true;
        }
        public void AddTransaction(Transaction transaction)
        {
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();
        }
        public void BalanceUpdateUserDetails(BalanceUpdateUserDetails balanceUpdate)
        {
            _dbContext.BalanceUpdateUserDetails.Add(balanceUpdate);
            _dbContext.SaveChanges();
        }

   
    }
}