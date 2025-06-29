using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignment1.Validation
{
    public class AdminOnlyValidation : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (!httpContext.User.Identity.IsAuthenticated)
                return false;
            return httpContext.User.IsInRole("Admin");
        }
    }
}