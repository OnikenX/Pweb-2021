using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pweb_2021.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<Imovel> Imoveis { get; set; }
        public List<Reserva> Reservas { get; set; }
        public List<Feedback> Feedbacks { get; set; }

        public string GestorId { get; set; }
        public ApplicationUser Gestor { get; set; }
        public List<ApplicationUser> Funcionarios { get; set; }
    }
    public class NewUser
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
