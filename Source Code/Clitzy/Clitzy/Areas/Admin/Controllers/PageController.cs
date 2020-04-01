using Clitzy.Models;
using Clitzy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clitzy.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class PageController : Controller
    {
        private VishnuworldEntities ocmde = new VishnuworldEntities();

        public ActionResult Index()
        {
            try
            {
                ViewBag.pages = ocmde.Pages.ToList();
                return View();
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Page", "Index"));
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                var page = ocmde.Pages.SingleOrDefault(p => p.Id == id);
                return View("Edit", page);
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Page", "Edit"));
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Page page)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentPage = ocmde.Pages.Find(page.Id);
                    currentPage.Plug = page.Plug;
                    currentPage.Title = page.Title;
                    currentPage.Status = page.Status;
                    currentPage.Detail = page.Detail;
                    ocmde.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View("Edit", page);
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Page", "Edit"));
            }
        }

        public ActionResult Status(int id)
        {
            try
            {
                var page = ocmde.Pages.SingleOrDefault(p => p.Id == id);
                page.Status = !page.Status;
                ocmde.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Page", "Status"));
            }
        }

    }
}