using Microsoft.AspNetCore.Http;
using Pweb_2021.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pweb_2021.ViewModels
{
    public class ImovelImgViewModel
    {
        public int ImovelImgId { get; set; }

        public string Description { get; set; }

        //caminho para a imagem
        [Required(ErrorMessage = "Please choose profile image")]
        [Display(Name = "Profile Picture")]
        public IFormFile Image { get; set; }

        [Required]
        public int ImovelId { get; set; }
        public Imovel Imovel { get; set; }
    }

}
