using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pweb_2021.Models
{
    public class HelperClass
    {
       public bool isAuth =false;
       public bool isAdmin = false;
       public string userId = "";

       public HelperClass(Controller context)
        {
            setValues(context);
        }
        public void setValues(Controller context)
        {
            using (context)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                   isAuth = true;
                    isAdmin = context.User.IsInRole("admin");
                    userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
                else
                {
                    isAuth = false;
                    isAdmin = false;
                }
            }
        }
    }
}
