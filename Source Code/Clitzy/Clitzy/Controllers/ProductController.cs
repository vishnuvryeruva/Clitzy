using Clitzy.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Clitzy.Helpers;

namespace Clitzy.Controllers
{
    public class ProductController : Controller
    {
        private VishnuworldEntities ocmde = new VishnuworldEntities();
        
        [HttpGet]
        public ActionResult Search(int page = 1, int pageSize = 9)
        {
            try
            {
                pageSize = int.Parse(ocmde.Settings.Find(9).Value);
                string keyword = Request.QueryString.Get("keyword");
                int categoryId = int.Parse(Request.QueryString.Get("category"));
                List<Product> listProducts = ocmde.Products.Where(p => p.Name.Contains(keyword) && p.CategoryId == categoryId && p.Status).ToList();
                var products = new List<Product>();
                listProducts.ForEach(p =>
                {
                    if (VendorHelper.checkExpires(p.VendorId))
                    {
                        products.Add(p);
                    }
                });
                PagedList<Product> model = new PagedList<Product>(products, page, pageSize);
                ViewBag.keyword = keyword;
                ViewBag.categoryId = categoryId;
                return View("Search", model);
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Product", "Search"));
            }
        }

        public ActionResult Category(int id, int page = 1, int pageSize = 9)
        {
            try
            {
                pageSize = int.Parse(ocmde.Settings.Find(9).Value);
                List<Product> listProducts = ocmde.Products.Where(p => p.CategoryId == id && p.Status).ToList();
                var products = new List<Product>();
                listProducts.ForEach(p =>
                {
                    if (VendorHelper.checkExpires(p.VendorId))
                    {
                        products.Add(p);
                    }
                });
                PagedList<Product> model = new PagedList<Product>(products, page, pageSize);
                ViewBag.category = ocmde.Categories.Find(id);
                TempData["categorySelected"] = ocmde.Categories.Find(id).Category2.Id;
                return View("Category", model);
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Product", "Category"));
            }
        }

        public ActionResult Expires()
        {
            return View("Expires");
        }

        public ActionResult Detail(int id)
        {
            try
            {
                var product = ocmde.Products.Find(id);
                if (!VendorHelper.checkExpires(product.VendorId))
                {
                    return RedirectToAction("Expires", "Product");
                }
                else
                {
                    product.Views = product.Views + 1;
                    ocmde.SaveChanges();
                    ViewBag.product = product;
                    ViewBag.relatedProducts = ocmde.Products.Where(p => p.Id != id && p.CategoryId == product.CategoryId && p.VendorId == product.VendorId).Take(6).ToList();
                    return View("Detail");
                }
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Product", "Detail"));
            }
        }
    }
}