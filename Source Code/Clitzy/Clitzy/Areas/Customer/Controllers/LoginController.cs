using Clitzy.Helpers;
using Clitzy.Models;
using Clitzy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clitzy.Areas.Customer.Controllers
{
    public class LoginController : Controller
    {
        private VishnuworldEntities ocmde = new VishnuworldEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Process(FormCollection fc)
        {
            try
            {
                string username = fc["username"];
                string password = fc["password"];
                var account = login(username, password);
                if (account == null)
                {
                    ViewBag.error = Resources.Customer.Invalid_Account;
                    return View("Index");
                }
                else
                {
                    SessionPersister.account = account;
                    return RedirectToAction("Index", "Orders");
                }
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Login", "Process"));
            }
        }

        public ActionResult SignOut()
        {
            try
            {
                SessionPersister.account = null;
                return RedirectToAction("Index", "Login");
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Login", "SignOut"));
            }
        }

        [CustomAuthorize(Roles = "Customer")]
        [HttpGet]
        public ActionResult Profile()
        {
            try
            {
                var account = (Account)SessionPersister.account;
                return View("Profile", account);
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Login", "Profile"));
            }
        }

        [CustomAuthorize(Roles = "Customer")]
        [HttpPost]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public ActionResult Profile(Account account)
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        {
            try
            {
                var loginedAccount = (Account)SessionPersister.account;
                
                var currentAccount = ocmde.Accounts.SingleOrDefault(a => a.Id == account.Id);

                if (account.Username != null && account.Username.Length > 0 && loginedAccount.Username != account.Username)
                {
                    if (Exists(account.Username))
                    {
                        ModelState.AddModelError("username", Resources.Customer.Username_exists);
                    }
                }

                if (account.Password != null && account.Password.Length != 0 && !PasswordHelper.IsValidPassword(account.Password))
                {
                    ModelState.AddModelError("Password", Resources.Vendor.Password_validate_message);
                }

                if (ModelState.IsValid)
                {
                    if (account.Password != null && account.Password.Length != 0)
                    {
                        currentAccount.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
                    }
                    currentAccount.Email = account.Email;
                    currentAccount.FullName = account.FullName;
                    currentAccount.Phone = account.Phone;
                    currentAccount.Username = account.Username;
                    ocmde.SaveChanges();
                    SessionPersister.account = ocmde.Accounts.Find(account.Id);
                    return RedirectToAction("Profile", "Login");
                }
                else
                {
                    return View("Profile", account);
                }
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Login", "Profile"));
            }
        }

        private bool Exists(string username)
        {
            return ocmde.Accounts.Count(a => a.Username.Equals(username)) > 0;
        }

        private Account login(string username, string password)
        {
            try
            {
                var account = ocmde.Accounts.SingleOrDefault(a => a.Username.Equals(username) && !a.IsAdmin);
                if (account != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(password, account.Password) && account.Status)
                    {
                        return account;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}