using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pweb_2021.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pweb_2021.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Imovel> Imoveis { get; set; }

        public DbSet<ImovelImg> ImovelImgs { get; set; }
        public DbSet<Reserva> Reservas{ get; set; }
    }
}
