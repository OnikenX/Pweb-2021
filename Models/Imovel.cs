using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pweb_2021.Models
{
    public class Imovel
    {

        public int ImovelId { get; set; }

        public string Descricao { get; set; }


        public List<ImovelImg> ImovelImgs { get;set;}
        public List<Reserva> Reservas { get;set;}

        //quem registou o imovel
        public string IdentityUserId { get; set; }
        public IdentityUser IdentityUser { get; set; }
    }

    public class ImovelImg {
        public int ImovelImgId { get; set; }
        
        //caminho para a imagem
        [Required]
        public string pathToImage { get; set; }
        

        [Required]
        public int ImovelId { get; set; }
        public Imovel Imovel { get; set; }
    }

    public class Reserva
    {
        public int ReservaId { get; set; }
       
        //datas
        [Required]
        public DateTime DataInicial{ get; set; }
        [Required]
        public DateTime DataFinal { get; set; }

        //imovel a reservar
        [Required]
        public int ImovelId { get; set; }
        public Imovel Imovel { get; set; }
        
        //quem reservou
        [Required]
        public string IdentityUserId { get; set; }
        public IdentityUser IdentityUser { get; set; }
    }
}
