using Clitzy.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Clitzy.Security;
using System.IO;
using Clitzy.Paypal;
using Clitzy.Helpers;

namespace Clitzy.Controllers
{
    public class VendorsController : Controller
    {
        private VishnuworldEntities ocmde = new VishnuworldEntities();

        public ActionResult Expires()
        {
            return View("Expires");
        }

        public ActionResult Detail(int id, int page = 1, int pageSize = 9)
        {
            try
            {
                if (!VendorHelper.checkExpires(id))
                {
                    return RedirectToAction("Expires", "Vendors");
                }
                else
                {
                    pageSize = int.Parse(ocmde.Settings.Find(9).Value);
                    var vendor = ocmde.Vendors.Find(id);
                    List<Product> listProducts = vendor.Products.Where(p => p.Status).ToList();
                    PagedList<Product> model = new PagedList<Product>(listProducts, page, pageSize);
                    ViewBag.vendor = vendor;
                    ViewBag.comments = vendor.Reviews.Where(r => r.VendorId == id).OrderByDescending(r => r.Id).ToList();
                    return View("Index", model);
                }
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Vendors", "Index"));
            }
        }

        [HttpGet]
        public ActionResult SendMessage(int id)
        {
            try
            {
                if (!VendorHelper.checkExpires(id))
                {
                    return RedirectToAction("Expires", "Vendors");
                }
                else
                {
                    if (SessionPersister.account == null)
                    {
                        return RedirectToAction("Login", "Customer");
                    }
                    else
                    {
                        ViewBag.vendor = ocmde.Vendors.Find(id);
                        return View("SendMessage", new Message() { VendorId = id });
                    }
                }
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Vendors", "SendMessage"));
            }
        }

        [HttpPost]
        public ActionResult SendMessage(Message message)
        {
            try
            {
                var customer = (Clitzy.Models.Account)SessionPersister.account;
                message.CustomerId = customer.Id;
                message.DateCreation = DateTime.Now;
                message.Status = false;
                ocmde.Messages.Add(message);
                ocmde.SaveChanges();
                TempData["message"] = Resources.Vendor.messages_sent_success;
                return RedirectToAction("SendMessage");
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Vendors", "SendMessage"));
            }
        }

        [HttpGet]
        public ActionResult GiveReview(int id)
        {
            try
            {
                if (!VendorHelper.checkExpires(id))
                {
                    return RedirectToAction("Expires", "Vendors");
                }
                else
                {
                    if (SessionPersister.account == null)
                    {
                        TempData["message"] = Resources.Vendor.You_need_login;
                        return RedirectToAction("Login", "Customer");
                    }
                    else
                    {
                        var customer = (Clitzy.Models.Account)SessionPersister.account;
                        if (!checkCustomerOfVendor(customer.Id, id))
                        {
                            TempData["message"] = Resources.Vendor.You_must_be_customer;
                            return RedirectToAction("Login", "Customer");
                        }
                        else
                        {
                            ViewBag.vendor = ocmde.Vendors.Find(id);
                            return View("GiveReview", new Review() { VendorId = id });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Vendors", "GiveReview"));
            }
        }

        [HttpPost]
        public ActionResult GiveReview(Review review)
        {
            try
            {
                var customer = (Clitzy.Models.Account)SessionPersister.account;
                review.CustomerId = customer.Id;
                review.DatePost = DateTime.Now;
                ocmde.Reviews.Add(review);
                ocmde.SaveChanges();
                TempData["message"] = Resources.Vendor.Your_review_sent_to_vendor;
                return RedirectToAction("GiveReview");
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Vendors", "GiveReview"));
            }
        }

        private bool checkCustomerOfVendor(int customerId, int vendorId)
        {
            return ocmde.Orders.Count(o => o.VendorId == vendorId && o.CustomerId == customerId) > 0;
        }

        public ActionResult Category(int id, int vendorId, int page = 1, int pageSize = 9)
        {
            try
            {
                if (!VendorHelper.checkExpires(vendorId))
                {
                    return RedirectToAction("Expires", "Vendors");
                }
                else
                {
                    pageSize = int.Parse(ocmde.Settings.Find(9).Value);
                    List<Product> listProducts = ocmde.Products.Where(p => p.CategoryId == id && p.Status).ToList();
                    PagedList<Product> model = new PagedList<Product>(listProducts, page, pageSize);
                    ViewBag.category = ocmde.Categories.Find(id);
                    TempData["categoryVendorSelected"] = ocmde.Categories.Find(id).Category2.Id;
                    ViewBag.vendor = ocmde.Vendors.Find(vendorId);
                    return View("Category", model);
                }
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Vendor", "Category"));
            }
        }

        public ActionResult MemberShip()
        {
            try
            {
                ViewBag.memberships = ocmde.MemberShips.Where(m => m.Status).ToList();
                ViewBag.PayPalSubmitUrl = ocmde.Settings.Find(11).Value;
                ViewBag.PayPalUsername = ocmde.Settings.Find(12).Value;
                ViewBag.ReturnUrl = ocmde.Settings.Find(13).Value;
                return View("MemberShip");
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Vendors", "MemberShip"));
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            try
            {
                return View("Register", new Vendor());
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Vendors", "Register"));
            }
        }

        [HttpPost]
        public ActionResult Register(Vendor vendor, HttpPostedFileBase logo)
        {
            try
            {
                if (vendor.Username != null && Exists(vendor.Username))
                {
                    ModelState.AddModelError("Username", Resources.Vendor.Username_already_exists);
                }

                if (vendor.Password != null && !PasswordHelper.IsValidPassword(vendor.Password))
                {
                    ModelState.AddModelError("Password", "Invalid Password");
                }

                if (logo != null && logo.ContentLength > 0 && !logo.ContentType.Contains("image"))
                {
                    ViewBag.errorPhoto = Resources.Vendor.Photo_Invalid;
                    return View("Register", vendor);
                }

                if (ModelState.IsValid)
                {
                    vendor.Password = BCrypt.Net.BCrypt.HashPassword(vendor.Password);
                    vendor.Status = true;
                    if (logo != null && logo.ContentLength > 0 && logo.ContentType.Contains("image"))
                    {
                        var fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetFileName(logo.FileName);
                        logo.SaveAs(Path.Combine(Server.MapPath("~/Content/User/Images"), fileName));
                        vendor.Logo = fileName;
                    }
                    else
                    {
                        vendor.Logo = "no-logo.jpg";
                    }
                    ocmde.Vendors.Add(vendor);

                    // Add Package Trial to new vendor
                    var membership = ocmde.MemberShips.Find(MemberShipHelper.TrialPackage);
                    MemberShipVendor memberShipVendor = new MemberShipVendor()
                    {
                        MemerShipId = membership.Id,
                        VendorId = vendor.Id,
                        Price = membership.Price,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddMonths(membership.Month)
                    };
                    ocmde.MemberShipVendors.Add(memberShipVendor);
                    ocmde.SaveChanges();

                    ocmde.SaveChanges();
                    return RedirectToAction("Index", "Login", new { Area = "Vendor" });
                }
                return View("Register", vendor);
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Vendors", "Register"));
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

        public ActionResult Success()
        {
            try
            {
                var result = PDTHolder.Success(Request.QueryString.Get("tx"));
                if (result != null)
                {
                    var membership = ocmde.MemberShips.Find(result.MemberShipId);
                    var vendor = (Clitzy.Models.Vendor)SessionPersister.account;
                    MemberShipVendor memberShipVendor = new MemberShipVendor()
                    {
                        MemerShipId = membership.Id,
                        VendorId = vendor.Id,
                        Price = membership.Price,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddMonths(membership.Month)
                    };
                    ocmde.MemberShipVendors.Add(memberShipVendor);
                    ocmde.SaveChanges();
                    ViewBag.msg = "Success";
                }
                else
                {
                    ViewBag.msg = "Error";
                }
                return View("Success");
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Vendors", "Success"));
            }
        }

    }
}