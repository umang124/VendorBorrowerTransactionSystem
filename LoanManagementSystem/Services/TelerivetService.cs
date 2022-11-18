using LoanManagementSystem.DataAccessLayer;
using LoanManagementSystem.Interface;
using System.Collections.Generic;
using System.Web.Configuration;
using Telerivet.Client;

namespace LoanManagementSystem.Services
{
    public class TelerivetService : ITelerivetService
    {
        public bool CheckifRequestDataIsForWithdrawAmount(string code, string data)
        {
            if (code == "4" && double.TryParse(data, out double result1))
            {
                return true;
            }
            return false;
        }

        public bool CheckifRequestDataIsRegister(string data)
        {
            if (!double.TryParse(data, out double result2))
            {
                return true;
            }
            return false;
        }
    }
}