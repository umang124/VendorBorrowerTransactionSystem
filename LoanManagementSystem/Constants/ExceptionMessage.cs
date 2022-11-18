using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoanManagementSystem.Constants
{
    public class ExceptionMessage
    {
        public const string WITHDRAW_PENDING = "Your Withdraw is on pending";
        public const string REGISTERED_SUCCES = "User registered successfully";
        public const string TRANSACTION_PENDING = "Your transaction is on pending";
        public const string PHONENUMBER_NOT_REGISTERED = "Your phonenumbr is not registered";
        public const string PINCODE_MUST_BE_A_NUMBER = "PinCode must be a number";
        public const string INVALID_PINCODE = "Invalid Pincode";
        public const string NOT_ENOUGH_BALANCE = "Your donot have enough balance!";
        public const string INVALID_USERTYPE = "Invalid UserType";
        public const string CANNOT_REGISTERED_AS_BANKER = "Cannot registered as banker";
        public const string CANNOT_REGISTERED_AS_ADMIN = "Cannot registered as banker";
        public const string PHONENUMBER_REGISTERED = "PhoneNumber is already registered!";
        public const string PINCODE_4_DIGIT = "Pincode must be a 4 digit number";
        public const string INVALID_PHONENUMBER = "Invalid Phonenumber";
        public const string RECEIVER_NUMBER_NOT_EXIST = "Receiver number doesnot exist";
        public const string VENDOR_CANNOT_CREATE_TRANSACTION = "Vendor_Cannot_Create_Transaction";
        public const string INVALID_ID = "Invalid Id";
    }
}