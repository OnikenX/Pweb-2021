using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Pweb_2021.Models
{
    public class ApplicationUser : IdentityUser
    {
       
        public List<Imovel> Imoveis { get; set; }
        public List<Reserva> Reservas { get; set; }
        public List<Feedback> Feedbacks { get; set; }
    }
}
