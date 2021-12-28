using System.ComponentModel.DataAnnotations;

namespace Pweb_2021.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        /// <summary>
        /// maximo 10, minimo 0
        /// </summary>
        [Required]
        [Range(0, 10)]
        public byte Estrelas { get; set; }
        
        [MaxLength(5000)]
        public string Comentario { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int ImovelId { get; set; }
        public Imovel Imovel { get; set; }
    }
}