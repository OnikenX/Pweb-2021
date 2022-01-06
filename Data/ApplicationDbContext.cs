using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pweb_2021.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Pweb_2021.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Imovel> Imoveis { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<ImovelImg> ImovelImgs { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        public DbSet<Test> tests { get; set; }
        //public DbSet<FuncGestor> FuncGestors { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Reserva>().HasOne(e => e.Imovel).WithMany(e => e.Reservas).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Imovel>().HasMany(e => e.Reservas).WithOne(e => e.Imovel).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ApplicationUser>()
                .HasMany(c => c.Funcionarios)
                .WithOne(c => c.Gestor);
            builder.Entity<ApplicationUser>()
            .HasOne(c => c.Gestor)
            .WithMany(c => c.Funcionarios);
        }
    }
}

