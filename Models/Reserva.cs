using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pweb_2021.Models
{
    public class Reserva
    {
        public int ReservaId { get; set; }

        //datas
        [Required]
        public DateTime DataInicial { get; set; }
        [Required]
        public DateTime DataFinal { get; set; }

        //para o gestor
        [Range(0, 10)]
        [Display(Name = "Avaliação")]
        public byte Avaliacao { get; set; }

        //0 - rejeitado
        //1 - em espera por confirmar
        //2 - confirmado
        //3 - com o cliente
        //4 - devolvido do cliente
        [Range(0, 4)]
        public byte Estado { get; set; }

        public string Estado_string()
        {
            switch (Estado)
            {
                case 0:
                    return "rejeitado";
                    break;
                case 1:
                    return "Em espera de confirmação";
                case 2:
                    return "Reserva Aceite";
                case 3:
                    return "Imovel entregado ao cliente";
                case 4:
                    return "Imovel devolvido ao funcionario";
                default:
                    return "Undefined";
            }
        }

        [Display(Name = "Comentário")]
        public string Comentario { get; set; }

        //imovel a reservar
        [Required]
        public int ImovelId { get; set; }
        public Imovel Imovel { get; set; }


        public List<Feedback> Feedbacks { get; set; }

        //quem reservou
        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
