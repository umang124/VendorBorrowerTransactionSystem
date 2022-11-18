using LoanManagementSystem.Constants;
using LoanManagementSystem.ExceptionCustom;
using LoanManagementSystem.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using Telerivet.Client;

namespace LoanManagementSystem.Controllers
{
    public class TelerevitController : Controller
    {
        private IUserService _userService;
        private ITransactionService _transactionService;
        private ITelerivetService _telerivetService;
        public TelerevitController(IUserService userService, ITransactionService transactionService, ITelerivetService telerivetService)
        {
            _userService = userService;
            _transactionService = transactionService;
            _telerivetService = telerivetService;
        }
        public ActionResult Create()
        {
            Dictionary<string, object> reply = new Dictionary<string, object>();
            Dictionary<string, object> result = new Dictionary<string, object>();

            TelerivetAPI tr = new TelerivetAPI(WebConfigurationManager.AppSettings["API_Key"]);
            Project project = tr.InitProjectById(WebConfigurationManager.AppSettings["ProjectId"]);

            string statusSecret = WebConfigurationManager.AppSettings["statusSecret"];

            if (Request["secret"] != statusSecret)
            {
                return RedirectToAction("Index", "Home");
            }

            if (Request["event"] == "incoming_message")
            {
                String datas = Request["content"];
                string fromNumber = Request["from_number"];
                string[] dataList = datas.Split(' ');
                var count = dataList.Count();
                string code = dataList[0];

                try
                {
                    if (count == 3)
                    {
                        if (_telerivetService.CheckifRequestDataIsForWithdrawAmount(code, dataList[1]))
                        {
                            // withdraw amount  
                            _transactionService.WithDrawAmount(Convert.ToDecimal(dataList[1]), dataList[2], fromNumber);
                            reply["content"] = ExceptionMessage.WITHDRAW_PENDING;
                            result["messages"] = new Object[] { reply };
                            return Json(result);
                        }
                        // register vendor/borrower
                        _userService.RegisterUser(dataList[0], dataList[1], dataList[2], fromNumber);
                        reply["content"] = ExceptionMessage.REGISTERED_SUCCES;
                        result["messages"] = new Object[] { reply };
                        return Json(result);
                    }
                    else if (count == 4)
                    {
                        //  Create Transaction
                        _transactionService.CreateTransaction(dataList[1], Convert.ToDecimal(dataList[2]), dataList[3], fromNumber);
                        reply["content"] = ExceptionMessage.TRANSACTION_PENDING;
                        result["messages"] = new Object[] { reply };
                        return Json(result);
                    }
                    reply["content"] = "Invalid Action";
                    result["messages"] = new Object[] { reply };
                    return Json(result);
                }
                catch (CustomException ex)
                {
                    reply["content"] = Convert.ToString(ex.Message);
                    result["messages"] = new Object[] { reply };
                    return Json(result);
                }
            }
            else
            {
                Response.StatusCode = 400;
                return Content("Unknown event");
            }
        }
    }
}