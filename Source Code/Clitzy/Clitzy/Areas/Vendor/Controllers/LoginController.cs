using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Clitzy.Models;
using Clitzy.Security;
using System.IO;
using Clitzy.Helpers;

namespace Clitzy.Areas.Vendor.Controllers
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
                var vendor = login(username, password);
                if (vendor == null)
                {
                    ViewBag.error = Resources.Customer.Invalid_Account;
                    return View("Index");
                }
                else
                {
                    SessionPersister.account = vendor;
                    return RedirectToAction("Index", "Category");
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

        [CustomAuthorize(Roles = "Vendor")]
        [HttpGet]
        public ActionResult Profile()
        {
            try
            {
                var vendor = (Clitzy.Models.Vendor)SessionPersister.account;
                return View("Profile", vendor);
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Login", "Profile"));
            }
        }

        [CustomAuthorize(Roles = "Vendor")]
        [HttpPost]
        public ActionResult Profile(Clitzy.Models.Vendor vendor, HttpPostedFileBase logo)
        {
            try
            {
                var loginedAccount = (Clitzy.Models.Vendor)SessionPersister.account;
                
                var currentVendor = ocmde.Vendors.SingleOrDefault(a => a.Id == vendor.Id);

                if (vendor.Username != null && vendor.Username.Length > 0 && loginedAccount.Username != vendor.Username)
                {
                    if (Exists(vendor.Username))
                    {
                        ModelState.AddModelError("username", Resources.Customer.Username_exists);
                    }
                }

                if (vendor.Password != null && vendor.Password.Length != 0 && !PasswordHelper.IsValidPassword(vendor.Password))
                {
                    ModelState.AddModelError("Password", Resources.Vendor.Password_validate_message);
                }

                if (logo != null && logo.ContentLength > 0 && !logo.ContentType.Contains("image"))
                {
                    ViewBag.errorPhoto = Resources.Vendor.Photo_Invalid;
                    return View("Profile", loginedAccount);
                }

                if (ModelState.IsValid)
                {
                    if (vendor.Password != null && vendor.Password.Length != 0)
                    {
                        currentVendor.Password = BCrypt.Net.BCrypt.HashPassword(vendor.Password);
                    }

                    if (logo != null && logo.ContentLength > 0 && logo.ContentType.Contains("image"))
                    {
                        logo.SaveAs(Path.Combine(Server.MapPath("~/Content/User/Images"), Path.GetFileName(logo.FileName)));
                        currentVendor.Logo = Path.GetFileName(logo.FileName);
                    }
                    currentVendor.Email = vendor.Email;
                    currentVendor.Name = vendor.Name;
                    currentVendor.Phone = vendor.Phone;
                    currentVendor.Address = vendor.Address;
                    currentVendor.Username = vendor.Username;
                    ocmde.SaveChanges();
                    SessionPersister.account = ocmde.Vendors.Find(vendor.Id);
                    return RedirectToAction("Profile", "Login");
                }
                else
                {
                    return View("Profile", vendor);
                }
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Login", "Profile"));
            }
        }

        private bool Exists(string username)
        {
            try
            {
                return ocmde.Vendors.Count(a => a.Username.Equals(username)) > 0;
            }
            catch
            {
                return false;
            }
        }

        private Clitzy.Models.Vendor login(string username, string password)
        {
            try
            {
                var vendor = ocmde.Vendors.SingleOrDefault(v => v.Username.Equals(username));
                if (vendor != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(password, vendor.Password) && vendor.Status && VendorHelper.checkExpires(vendor.Id))
                    {
                        return vendor;
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