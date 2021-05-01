using OHD.Data;
using OHD.Data.Constant;
using OHD.Dto;
using OHD.Interface;
using OHD.Models;
using OHD.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace OHD.Service
{
    public class AccountService : IAccount
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }
        public Account AddAccount(AccountViewModel accountViewModel)
        {
            var hashPassword = BCrypt.Net.BCrypt.HashPassword(accountViewModel.Account.Password, BCrypt.Net.BCrypt.GenerateSalt());
            accountViewModel.Account.Password = hashPassword;
            _context.Account.Add(accountViewModel.Account);
            _context.SaveChanges();

            return accountViewModel.Account;
        }

        public bool DeleteAccount(int accountId)
        {
            var account = _context.Account.Find(accountId);
            _context.Account.Remove(account);
            _context.SaveChanges();

            return true;
        }

        public Account EditAccount(int accountId, AccountViewModel accountViewModel)
        {
            var account = _context.Account.Find(accountId);
            account.Username = accountViewModel.Account.Username;
            account.FullName = accountViewModel.Account.FullName;
            //account.Password = BCrypt.Net.BCrypt.HashPassword(accountViewModel.Account.Password, BCrypt.Net.BCrypt.GenerateSalt());
            account.RoleId = accountViewModel.Account.RoleId;
            account.Status = accountViewModel.Account.Status;
            _context.Account.Update(account);
            _context.SaveChanges();

            return account;
        }

        public Account EditProfile(Claim username, Account account)
        {
            var currentAccount = _context.Account.SingleOrDefault(c => c.Username.Equals(username.Value));            
            currentAccount.Password = account.Password;
            if (!string.IsNullOrEmpty(account.Password))
            {
                currentAccount.Password = BCrypt.Net.BCrypt
                    .HashPassword(account.Password, BCrypt.Net.BCrypt.GenerateSalt());
            }
            currentAccount.FullName = account.FullName;
            currentAccount.Email = account.Email;
            _context.Account.Update(currentAccount);
            _context.SaveChanges();

            return currentAccount;
        }

        public IQueryable<ListAccountResponse> GetListAccount()
        {
            var lstAccount = from a in _context.Account
                             join r in _context.Role
                             on a.RoleId equals r.Id
                             where a.RoleId != RoleConstants.ROLE_HEAD_OFFICE
                             select new ListAccountResponse
                             {
                                 Id = a.Id,
                                 Email = a.Email,
                                 FullName = a.FullName,
                                 Password = a.Password,
                                 RoleId = a.RoleId,
                                 RoleName = r.Name,
                                 Status = a.Status,
                                 Username = a.Username
                             };
            return lstAccount;
        }
    }
}
