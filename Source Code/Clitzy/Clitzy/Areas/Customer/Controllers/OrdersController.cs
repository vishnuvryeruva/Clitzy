using Clitzy.Models;
using Clitzy.Models.ViewModels;
using Clitzy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clitzy.Areas.Customer.Controllers
{
    [CustomAuthorize(Roles = "Customer")]
    public class OrdersController : Controller
    {
        private VishnuworldEntities ocmde = new VishnuworldEntities();

        public ActionResult Index()
        {
            try
            {
                var customer = (Clitzy.Models.Account)SessionPersister.account;
                ViewBag.orders = ocmde.Orders.Where(o => o.CustomerId == customer.Id).OrderByDescending(o => o.Id).ToList();
                return View();
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Orders", "Index"));
            }
        }

        public ActionResult Detail(int id)
        {
            try
            {
                ViewBag.order = ocmde.Orders.Find(id);
                return View("Detail");
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Orders", "Detail"));
            }
        }

    }
}