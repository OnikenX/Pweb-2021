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
        public bool isAuth { get; private set; }
        public bool isAdmin { get; private set; }

        public bool isGestor { get; private set; }
        public bool isFunc { get; private set; }
        public bool isCliente { get; private set; }

        public string userId = "";
        public int extraId1 = 0;
        public int extraId2 = 0;


        public HelperClass(Controller context)
        {
            setValues(context);
        }

        public static string getUserId(Controller context)
        {
            return context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        public void setValues(Controller context)
        {
            using (context)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    isAuth = true;
                    isAdmin = context.User.IsInRole(Statics.Roles.ADMIN);
                    isGestor = context.User.IsInRole(Statics.Roles.GESTOR);
                    isFunc = context.User.IsInRole(Statics.Roles.FUNCIONARIO);
                    isCliente = context.User.IsInRole(Statics.Roles.CLIENTE);
                    userId = getUserId(context);
                }
                else
                {
                    isAuth = false;
                    isAdmin = false;
                    isGestor = false;
                    isFunc = false;
                    isCliente = false;
                }
            }
        }
    }
}
