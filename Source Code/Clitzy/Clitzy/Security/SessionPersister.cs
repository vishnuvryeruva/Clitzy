using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clitzy.Security
{
    public static class SessionPersister
    {
        public static object account
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;
                var sessionVar = HttpContext.Current.Session["account"];
                if (sessionVar != null)
                    return sessionVar as object;
                return null;
            }
            set
            {
                HttpContext.Current.Session["account"] = value;
            }
        }
    }
}