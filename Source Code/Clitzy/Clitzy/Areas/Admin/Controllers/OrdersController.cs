using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Clitzy.Security;
using Clitzy.Models.ViewModels;
using Clitzy.Models;

namespace Clitzy.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private VishnuworldEntities ocmde = new VishnuworldEntities();

        public ActionResult Index()
        {
            try
            {
                ViewBag.orders = ocmde.Orders.OrderByDescending(o => o.Id).ToList();
                return View();
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Orders", "Index"));
            }
        }


    }
}