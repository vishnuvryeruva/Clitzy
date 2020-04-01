using Clitzy.Models;
using Clitzy.Models.ViewModels;
using Clitzy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clitzy.Areas.Vendor.Controllers
{
    [CustomAuthorize(Roles = "Vendor")]
    public class ReviewController : Controller
    {
        private VishnuworldEntities ocmde = new VishnuworldEntities();

        public ActionResult Index()
        {
            try
            {
                var vendor = (Clitzy.Models.Vendor)SessionPersister.account;
                ViewBag.reviews = ocmde.Reviews.Where(r => r.VendorId == vendor.Id).OrderByDescending(r => r.Id).ToList();
                return View();
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Review", "Index"));
            }
        }

        public ActionResult Customer(int id)
        {
            try
            {
                var vendor = (Clitzy.Models.Vendor)SessionPersister.account;
                ViewBag.reviews = ocmde.Reviews.Where(r => r.VendorId == vendor.Id && r.CustomerId == id).OrderByDescending(r => r.Id).ToList();
                return View("Index");
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Review", "Index"));
            }
        }

    }
}