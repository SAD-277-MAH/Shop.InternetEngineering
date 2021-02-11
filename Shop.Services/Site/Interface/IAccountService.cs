using Shop.Common.ReturnMessage;
using Shop.Data.Models;
using Shop.Data.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Site.Interface
{
    public interface IAccountService
    {
        Task<AccountReturnMessage> Register(RegisterViewModel viewModel);
        Task<AccountReturnMessage> Login(LoginViewModel viewModel);
        Task<string> GetActivateEmailToken(User user);
        Task<bool> ActivateEmail(string userName, string token);
        Task<string> GetChangePasswordToken(User user);
        Task<string> ResetPassword(string userName, string token);
    }
}
