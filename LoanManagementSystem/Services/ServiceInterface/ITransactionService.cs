using LoanManagementSystem.GetModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanManagementSystem.Interface
{
    public interface ITransactionService
    {
        ICollection<GetTransaction> GetUnApprovedTransactions();
        ICollection<GetTransaction> GetApprovedTransactions();
        ICollection<GetTransaction> GetRejectedTransactions();
        Task<bool> ApproveStatus(Guid transactionId, string bankerPhoneNumber);
        Task<bool> RejectStatus(Guid transactionId, string bankerPhoneNumber);
        ICollection<GetBalanceUpdate> GetUnApprovedWithdraw();
        ICollection<GetBalanceUpdate> GetApprovedWithdraw();
        ICollection<GetBalanceUpdate> GetRejectedWithdraw();
        ICollection<GetBalanceUpdate> GetDetailsOfBalanceUpdatedByBanker();
        Task RejectWithdraw(string getBankerMobileNumber, Guid b_id);
        Task ApproveWithdraw(string getBankerMobileNumber, Guid b_id);
        void InsertBalanceDetails(Guid id, decimal amountAdded, Guid BankerUserId);
        bool WithDrawAmount(decimal amount, string u_pinCode, string fromNumber);
        bool CreateTransaction(string toNumber, decimal amount, string u_pinCode, string fromNumber);
    }
}
