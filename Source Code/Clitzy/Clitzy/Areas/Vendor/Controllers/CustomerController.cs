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
    public class CustomerController : Controller
    {
        private VishnuworldEntities ocmde = new VishnuworldEntities();

        public ActionResult Index()
        {
            try
            {
                var vendor = (Clitzy.Models.Vendor)SessionPersister.account;
                var customerIds = ocmde.Vendors.Find(vendor.Id).Orders.Select(o => o.CustomerId).Distinct().ToList();
                var customers = new List<Account>();
                customerIds.ForEach(id => {
                    customers.Add(ocmde.Accounts.Find(id));
                });
                ViewBag.customers = customers;
                return View();
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Customer", "Index"));
            }
        }

        

    }
}