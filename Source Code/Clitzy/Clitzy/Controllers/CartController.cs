using Clitzy.Helpers;
using Clitzy.Models;
using Clitzy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OctopusCodesMultiVendor.Controllers
{
    public class CartController : Controller
    {
        private VishnuworldEntities ocmde = new VishnuworldEntities();

        public ActionResult Index()
        {
            try
            {
                return View("Index");
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Home", "Index"));
            }
        }

        public ActionResult Remove(int id)
        {
            List<Item> cart = (List<Item>)Session["cart"];
            int index = Exists(id, cart);
            cart.RemoveAt(index);
            Session["cart"] = cart;
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Update(FormCollection fc)
        {
            int productId = Convert.ToInt32(fc["productId"]);
            List<Item> cart = (List<Item>)Session["cart"];
            int index = Exists(productId, cart);
            cart[index].quantity = Convert.ToInt32(fc["quantity"]);
            Session["cart"] = cart;
            return RedirectToAction("Index");
        }

        public ActionResult Buy(int id)
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
                    if (Session["cart"] == null)
                    {
                        List<Item> cart = new List<Item>();
                        cart.Add(new Item()
                        {
                            product = product,
                            quantity = 1
                        });
                        Session["cart"] = cart;
                    }
                    else
                    {
                        List<Item> cart = (List<Item>)Session["cart"];
                        int index = Exists(id, cart);
                        if (index == -1)
                        {
                            cart.Add(new Item()
                            {
                                product = product,
                                quantity = 1
                            });
                        }
                        else
                        {
                            cart[index].quantity++;
                        }
                        Session["cart"] = cart;
                    }
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Home", "Index"));
            }
        }

        private int Exists(int productId, List<Item> cart)
        {
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].product.Id == productId)
                {
                    return i;
                }
            }
            return -1;
        }

        public ActionResult Save()
        {
            try
            {
                if (SessionPersister.account == null)
                {
                    return RedirectToAction("Index", "Login", new { Area = "Customer" });
                }
                else
                {
                    var account = SessionPersister.account;
                    if (account is Account && !((Clitzy.Models.Account)account).IsAdmin)
                    {
                        var customer = (Clitzy.Models.Account)account;
                        List<Item> cart = (List<Item>)Session["cart"];
                        var vendorIds = cart.Select(i => i.product.VendorId).Distinct().ToList();
                        vendorIds.ForEach(id =>
                        {

                            var currentVendor = ocmde.Vendors.Find(id);

                            // Create new order
                            Order order = new Order()
                            {
                                CustomerId = customer.Id,
                                DateCreation = DateTime.Now,
                                Name = Resources.Vendor.New_Order_for_Vendor + " " + currentVendor.Name,
                                OrderStatusId = 1,
                                VendorId = id
                            };
                            ocmde.Orders.Add(order);
                            ocmde.SaveChanges();

                            // Create order details
                            cart.Where(i => i.product.VendorId == id).ToList().ForEach(i =>
                            {
                                OrdersDetail ordersDetail = new OrdersDetail()
                                {
                                    OrderId = order.Id,
                                    Price = i.product.Price,
                                    Quantity = i.quantity,
                                    ProductId = i.product.Id
                                };
                                ocmde.OrdersDetails.Add(ordersDetail);
                                ocmde.SaveChanges();
                            });

                        });

                        // Remove Cart
                        Session.Remove("Cart");

                        return RedirectToAction("Index", "Orders", new { Area = "Customer" });
                    }
                    return RedirectToAction("Index", "Login", new { Area = "Customer" });
                }
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Home", "Index"));
            }
        }

    }
}