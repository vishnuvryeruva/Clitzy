using Clitzy.Helpers;
using Clitzy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OctopusCodesMultiVendor.Controllers
{
    public class CustomersController : Controller
    {
        private VishnuworldEntities ocmde = new VishnuworldEntities();

        [HttpGet]
        public ActionResult Register()
        {
            try
            {
                return View("Register", new Account());
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Customers", "Register"));
            }
        }

        [HttpPost]
        public ActionResult Register(Account account)
        {
            try {
                if (account.Username != null && account.Username.Length > 0)
                {
                    if (Exists(account.Username))
                    {
                        ModelState.AddModelError("username", Resources.Vendor.Username_already_exists);
                    }
                }

                if (account.Password != null && account.Password.Length != 0 && !PasswordHelper.IsValidPassword(account.Password))
                {
                    ModelState.AddModelError("Password", Resources.Vendor.Password_validate_message);
                }

                if (ModelState.IsValid)
                {
                    account.IsAdmin = false;
                    account.Status = true;
                    account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
                    ocmde.Accounts.Add(account);
                    ocmde.SaveChanges();
                    return RedirectToAction("Index", "Login", new { Area = "Customer" });
                }
                else
                {
                    return View("Register",account);
                }
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Customers", "Register"));
            }
        }

        private bool Exists(string username)
        {
            return ocmde.Accounts.Count(a => a.Username.Equals(username) && !a.IsAdmin) > 0;
        }
    }
}