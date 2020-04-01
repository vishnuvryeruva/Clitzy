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
    public class MemberShipController : Controller
    {
        private VishnuworldEntities ocmde = new VishnuworldEntities();

        public ActionResult Index()
        {
            try
            {
                var vendor = (Clitzy.Models.Vendor)SessionPersister.account;
                ViewBag.memberShipVendors = vendor.MemberShipVendors.OrderByDescending(m => m.Id);
                return View();
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "MemberShip", "Index"));
            }
        }

    }
}