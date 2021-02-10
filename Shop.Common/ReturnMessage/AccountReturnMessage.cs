using Shop.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Common.ReturnMessage
{
    public class AccountReturnMessage
    {
        public AccountReturnMessage(bool status, User user, List<string> errorMessages)
        {
            Status = status;
            User = user;
            ErrorMessages = errorMessages;
        }

        public bool Status { get; set; }
        public User User { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}
