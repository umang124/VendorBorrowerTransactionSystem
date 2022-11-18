using LoanManagementSystem.Data;
using LoanManagementSystem.DTOs;
using LoanManagementSystem.GetModel;
using LoanManagementSystem.Interface;
using LoanManagementSystem.Models;
using LoanManagementSystem.Services;
using Microsoft.Ajax.Utilities;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using Telerivet.Client;

namespace LoanManagementSystem.Controllers
{
    public class UserController : Controller
    {
        IUserService _userService;
        ITransactionService _transactionService;
        public UserController(IUserService userService, ITransactionService transactionService)
        {
            _userService = userService;
            _transactionService = transactionService;
        }

        [HttpGet]
        public ActionResult UpdateBalance(Guid id)
        {
            var getUser = _userService.GetAccountCredential(id);
            UpdateUserBalanceDTO updateUserBalanceDTO = new UpdateUserBalanceDTO();
            updateUserBalanceDTO.Balance = 0;
            updateUserBalanceDTO.Id = getUser.UserId;
            return View(updateUserBalanceDTO);
        }

        [HttpPost]
        public ActionResult UpdateBalance(UpdateUserBalanceDTO updateUserBalance)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Guid id = updateUserBalance.Id;
            var bankerMobileNumber = User.Identity.Name;

            decimal amountAdded = updateUserBalance.Balance;

            var getPreviousBalance = _userService.GetAccountCredential(id);
            decimal previousBalance = getPreviousBalance.Balance;
            updateUserBalance.Balance = updateUserBalance.Balance + previousBalance;
            _userService.UpdateBalance(id, updateUserBalance);


            var getBanker = _userService.GetAccountCredentialByNumber(bankerMobileNumber);

            _transactionService.InsertBalanceDetails(id, amountAdded, getBanker.UserId);

            return RedirectToAction("GetUsers", "User");
        }

        [Authorize(Roles = "Banker")]
        [HttpGet]
        public ActionResult GetUsers()
        {
            var getUsers = _userService.GetUsers();
            return View(getUsers);
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (User.IsInRole("Admin") || User.IsInRole("Banker"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult GetBankers()
        {
            var getBankers = _userService.GetBankers();
            return View(getBankers);
        }

        [HttpPost]
        public ActionResult Login(LoginDTO login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var getUserId = _userService.GetAccountCredentialByNumber(login.PhoneNumber);

            if (getUserId == null)
            {
                ModelState.AddModelError("", "PhoneNumber not registered");
                return View();
            }

            Guid userId = getUserId.UserId;

            var getUser = _userService.GetLoginCredentials(userId);
            Console.WriteLine();


            if (!_userService.VerifyPasswordHash(login.Password, getUser.PasswordHash, getUser.PasswordSalt))
            {
                ModelState.AddModelError("", "Password Incorrect");
                return View();
            }


            FormsAuthentication.SetAuthCookie(getUserId.PhoneNumber, false);

            User user = _userService.GetUser(userId);
            Session["u_name"] = user.Name;
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult CreateBanker()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateBanker(RegisterDTO register)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var checkPhoneNumber = _userService.CheckAccountCredentialByNUmber(register.PhoneNumber);
            if (checkPhoneNumber == true)
            {
                ModelState.AddModelError("", "PhoneNumber is already registered!");
                return View();
            }

            _userService.AddBanker(register);

            return RedirectToAction("GetBankers", "User");
        }

        [HttpGet]
        public ActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateAdmin(RegisterDTO register)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var checkPhoneNumber = _userService.CheckAccountCredentialByNUmber(register.PhoneNumber);
            if (checkPhoneNumber == true)
            {
                ModelState.AddModelError("", "PhoneNumber is already registered!");
                return View();
            }

            _userService.AddAdmin(register);

            return RedirectToAction("GetBankers", "User");
        }

        [Authorize(Roles = "Banker")]
        [HttpGet]
        public ActionResult GetUnApprovedWithdraw()
        {
            var getUnApprovedWithdraw = _transactionService.GetUnApprovedWithdraw();
            return View(getUnApprovedWithdraw);
        }

        [Authorize(Roles = "Banker")]
        [HttpGet]
        public ActionResult GetApprovedWithdraw()
        {
            var getApprovedWithdraw = _transactionService.GetApprovedWithdraw();
            return View(getApprovedWithdraw);
        }

        [Authorize(Roles = "Banker")]
        [HttpGet]
        public ActionResult GetRejectedWithdraw()
        {
            var getRejectedWithdraw = _transactionService.GetRejectedWithdraw();
            return View(getRejectedWithdraw);
        }

        [Authorize(Roles = "Banker")]
        [HttpPost]
        public async Task<ActionResult> ApproveWithdraw()
        {
            string getBankerMobileNumber = User.Identity.Name;
            Guid b_id = Guid.Parse(Request.Form["b_id"]);

            await _transactionService.ApproveWithdraw(getBankerMobileNumber, b_id);
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Banker")]
        [HttpPost]
        public async Task<ActionResult> RejectWithdraw()
        {
            string getBankerMobileNumber = User.Identity.Name;
            Guid b_id = Guid.Parse(Request.Form["b_id"]);

            await _transactionService.RejectWithdraw(getBankerMobileNumber, b_id);
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult GetDetailsOfBalanceUpdatedByBanker()
        {
            var getDetails = _transactionService.GetDetailsOfBalanceUpdatedByBanker();
            return View(getDetails);
        }
    }
}