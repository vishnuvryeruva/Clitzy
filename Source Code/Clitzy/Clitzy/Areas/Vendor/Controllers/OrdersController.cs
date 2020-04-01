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
    public class OrdersController : Controller
    {
        private VishnuworldEntities ocmde = new VishnuworldEntities();

        public ActionResult Index()
        {
            try
            {
                var vendor = (Clitzy.Models.Vendor)SessionPersister.account;
                ViewBag.orders = ocmde.Orders.Where(o => o.VendorId == vendor.Id).OrderByDescending(o => o.Id).ToList();
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
                ViewBag.orderId = id;
                ViewBag.payments = ocmde.Payments.Where(p => p.Status).ToList();
                ViewBag.orderStatus = ocmde.OrderStatus.Where(os => os.Status).ToList();
                return View("Detail");
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Orders", "Detail"));
            }
        }

        public ActionResult Customer(int id)
        {
            try
            {
                var vendor = (Clitzy.Models.Vendor)SessionPersister.account;
                ViewBag.orders = ocmde.Orders.Where(o => o.VendorId == vendor.Id && o.CustomerId == id).OrderByDescending(o => o.Id).ToList();
                return View("Index");
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Orders", "Customer"));
            }
        }

        [HttpPost]
        public ActionResult Update(FormCollection fc)
        {
            int id = int.Parse(fc["id"]);
            int paymentId = int.Parse(fc["payment"]);
            int orderStatusId = int.Parse(fc["orderStatus"]);
            var order = ocmde.Orders.Find(id);
            order.PaymentId = paymentId;
            order.OrderStatusId = orderStatusId;
            ocmde.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}