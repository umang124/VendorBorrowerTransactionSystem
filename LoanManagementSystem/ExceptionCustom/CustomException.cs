using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoanManagementSystem.ExceptionCustom
{
    [Serializable]
    public class CustomException : Exception
    {
        public CustomException(string name) : base(name)
        {

        }
    }
}