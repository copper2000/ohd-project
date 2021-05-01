using OHD.Dto;
using OHD.Models;
using OHD.Models.ViewModels;
using System.Linq;
using System.Security.Claims;

namespace OHD.Interface
{
    public interface IAccount
    {
        IQueryable<ListAccountResponse> GetListAccount();
        Account AddAccount(AccountViewModel accountViewModel);
        Account EditAccount(int accountId, AccountViewModel accountViewModel);
        bool DeleteAccount(int accountId);

        Account EditProfile(Claim username, Account account);
    }
}
