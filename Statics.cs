using Pweb_2021.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Pweb_2021.Statics
{
    public class Misc
    {
        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] bytes = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            return bytes;
        }
        
    }

    public class Roles
    {
        public static string[] getRoles()
        {
            return new string[] { ADMIN, FUNCIONARIO, GESTOR, CLIENTE };
        }

        public const string ADMIN = "admin";
        public const string FUNCIONARIO = "func";
        public const string GESTOR = "gestor";
        public const string CLIENTE = "cliente";
    }

    class Root
    {
        public const string user = "root";
        public const string mail = "root@root.com";
        public const string password = "Root10@";
    }
}

