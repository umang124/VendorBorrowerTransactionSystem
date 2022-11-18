using LoanManagementSystem.Data;
using LoanManagementSystem.Interface;
using LoanManagementSystem.Services;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using Telerivet.Client;

namespace LoanManagementSystem.Controllers
{
    [Authorize(Roles = "Admin, Banker")]
    public class TransactionController : Controller
    {
        ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        
        [HttpGet]
        public ActionResult GetUnApprovedTransactions()
        {
            var getTransactions = _transactionService.GetUnApprovedTransactions();
            return View(getTransactions);
        }
      
        [HttpGet]
        public ActionResult GetApprovedTransactions()
        {
            var getTransactions = _transactionService.GetApprovedTransactions();
            return View(getTransactions);
        }
   
        [HttpGet]
        public ActionResult GetRejectedTransactions()
        {
            var getTransactions = _transactionService.GetRejectedTransactions();
           
            return View(getTransactions);
        }

        [Authorize(Roles = "Banker")]
        [HttpPost]
        public async Task<ActionResult> ApproveStatus()
        {
            Guid transactionId = Guid.Parse(Request.Form["t_id"]);
            string bankerPhoneNumber = User.Identity.Name;
            await _transactionService.ApproveStatus(transactionId, bankerPhoneNumber);

            return RedirectToAction("GetUnApprovedTransactions", "Transaction");
        }

        [Authorize(Roles = "Banker")]
        [HttpPost]
        public async Task<ActionResult> RejectStatus()
        {
            Guid transactionId = Guid.Parse(Request.Form["t_id"]);
            var bankerPhoneNumber = User.Identity.Name;

            await _transactionService.RejectStatus(transactionId, bankerPhoneNumber);

            return RedirectToAction("GetUnApprovedTransactions", "Transaction");
        }
    }
}