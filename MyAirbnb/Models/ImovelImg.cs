using System.ComponentModel.DataAnnotations;

namespace Pweb_2021.Models
{
    public class ImovelImg {
        public int ImovelImgId { 
            get; set; }
        
        public string Description { get; set; }

        //caminho para a imagem
        [Required]
        public string pathToImage { get; set; }
        
        [Required]
        public int ImovelId { get; set; }
        public Imovel Imovel { get; set; }
    }
}

