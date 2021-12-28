using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pweb_2021.Statics
{
    class Roles
    {
        public readonly string[] roles = new string[] { ADMIN, FUNCIONARIO };

        public const string ADMIN = "admin";
        public const string FUNCIONARIO = "func";
    }

    class Root
    {
        public const string user = "root";
        public const string mail = "root@root.com";
        public const string password = "root";
    }
}

