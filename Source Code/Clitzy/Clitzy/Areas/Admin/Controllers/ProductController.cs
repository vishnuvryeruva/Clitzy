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
    public class ProductController : Controller
    {
        private VishnuworldEntities ocmde = new VishnuworldEntities();

        public ActionResult Index()
        {
            try
            {
                ViewBag.products = ocmde.Products.OrderByDescending(o => o.Id).ToList();
                return View();
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Product", "Index"));
            }
        }

        public ActionResult Status(int id)
        {
            try
            {
                var product = ocmde.Products.SingleOrDefault(p => p.Id == id);
                product.Status = !product.Status;
                ocmde.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Product", "Status"));
            }
        }

    }
}