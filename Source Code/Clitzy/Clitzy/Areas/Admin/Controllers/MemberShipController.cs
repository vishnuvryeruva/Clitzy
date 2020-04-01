using Clitzy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Clitzy.Security;
using Clitzy.Models.ViewModels;
using Clitzy.Helpers;

namespace Clitzy.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class MemberShipController : Controller
    {
        private VishnuworldEntities ocmde = new VishnuworldEntities();

        public ActionResult Index()
        {
            try
            {
                ViewBag.memberships = ocmde.MemberShips.ToList();
                return View();
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "MemberShip", "Index"));
            }
        }

        [HttpGet]
        public ActionResult Add()
        {
            try
            {
                return View("Add", new Clitzy.Models.MemberShip());
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "MemberShip", "Add"));
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Clitzy.Models.MemberShip memberShip)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ocmde.MemberShips.Add(memberShip);
                    ocmde.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View("Add", memberShip);
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "MemberShip", "Add"));
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                if (id == MemberShipHelper.TrialPackage)
                {
                    TempData["error"] = Resources.Customer.Can_not_delete_Trial_Package;
                    return RedirectToAction("Index");
                }
                else
                {
                    var membership = ocmde.MemberShips.SingleOrDefault(m => m.Id == id);
                    ocmde.MemberShips.Remove(membership);
                    ocmde.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "MemberShip", "Delete"));
            }
        }

        public ActionResult Status(int id)
        {
            try
            {
                var membership = ocmde.MemberShips.SingleOrDefault(m => m.Id == id);
                membership.Status = !membership.Status;
                ocmde.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "MemberShip", "Status"));
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                var membership = ocmde.MemberShips.SingleOrDefault(m => m.Id == id);
                return View("Edit", membership);
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "MemberShip", "Edit"));
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Clitzy.Models.MemberShip memberShip)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentMemberShip = ocmde.MemberShips.Find(memberShip.Id
    );
                    currentMemberShip.Name = memberShip.Name;
                    currentMemberShip.Price = memberShip.Price;
                    currentMemberShip.Status = memberShip.Status;
                    currentMemberShip.Description = memberShip.Description;
                    currentMemberShip.Month = memberShip.Month;
                    ocmde.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View("Edit", memberShip);
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "MemberShip", "Edit"));
            }
        }

    }
}