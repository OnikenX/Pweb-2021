using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pweb_2021.Models
{
    public class Imovel
    {
        public int ImovelId { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        [Display(Name = "Descrição")]
        [MaxLength(5000)]
        public string Descricao { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "O valor não pode ser negativo.")]
        [Display(Name = "Preço")]
        public int Preco { get; set; }
        public List<ImovelImg> ImovelImgs { get;set;}
        public List<Reserva> Reservas { get;set;}
        

        //quem registou o imovel
        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        
    }
}
