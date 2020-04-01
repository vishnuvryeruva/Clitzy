using Clitzy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clitzy.Helpers
{
    public class VendorHelper
    {
        private static VishnuworldEntities ocmde = new VishnuworldEntities();

        public static bool checkExpires(int vendorId)
        {
            try
            {
                var lastMemberShip = ocmde.MemberShipVendors.Where(m => m.VendorId == vendorId).OrderByDescending(m => m.Id).First();
                return lastMemberShip.EndDate >= DateTime.Now;
            }
            catch
            {
                return false;
            }
        }
    }
}