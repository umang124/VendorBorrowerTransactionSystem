using LoanManagementSystem.GetModel;
using LoanManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanManagementSystem.Repositories.RepositoryInterface
{
    public interface ITransactionRepository
    {
        ICollection<GetTransaction> GetUnApprovedTransactions();
        ICollection<GetTransaction> GetApprovedTransactions();
        ICollection<GetTransaction> GetRejectedTransactions();
        Transaction GetTransaction(Guid transactionId);
        ICollection<GetBalanceUpdate> GetUnApprovedWithdraw();
        ICollection<GetBalanceUpdate> GetApprovedWithdraw();
        ICollection<GetBalanceUpdate> GetRejectedWithdraw();
        ICollection<GetBalanceUpdate> GetDetailsOfBalanceUpdatedByBanker();
        Task<bool> RejectWithdrawManager(string getBankerMobileNumber, Guid b_id);
        Task<bool> ApproveWithdrawManager(string getBankerMobileNumber, Guid b_id);
        bool InsertBalanceDetails(Guid id, decimal amountAdded, Guid BankerUserId);
        Task<bool> ApproveStatusDataManager(Guid transactionId, string bankerPhoneNumber);
        Task<bool> RejectStatusDataManager(Guid transactionId, string bankerPhoneNumber);
        void AddTransaction(Transaction transaction);
        void BalanceUpdateUserDetails(BalanceUpdateUserDetails balanceUpdate);
    }
}
