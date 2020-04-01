using Clitzy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OctopusCodesMultiVendor.Controllers
{
    public class PageController : Controller
    {
        private VishnuworldEntities ocmde = new VishnuworldEntities();

        [HttpGet]
        public ActionResult Detail(string id)
        {
            try
            {
                ViewBag.page = ocmde.Pages.SingleOrDefault(p => p.Plug.Equals(id));
                return View("Index");
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Page", "Detail"));
            }
        }

        
    }
}