using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanManagementSystem.Interface
{
    public interface ITelerivetService
    {

        bool CheckifRequestDataIsForWithdrawAmount(string code, string data);
        bool CheckifRequestDataIsRegister(string data);
    }
}
