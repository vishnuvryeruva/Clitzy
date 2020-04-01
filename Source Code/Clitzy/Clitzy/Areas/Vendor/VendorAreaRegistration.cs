using System.Web.Mvc;

namespace Clitzy.Areas.Vendor
{
    public class VendorAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Vendor";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
           context.MapRoute(
                "Vendor_default",
                "Vendor/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Clitzy.Areas.Vendor.Controllers" }
            );
        }
    }
}