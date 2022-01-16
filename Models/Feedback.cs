using System;
using System.Collections.Generic;
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

        public int ReservaId { get; set; }
        public Reserva Reserva{ get; set; }


        public string MakeStarts()
        {
            return MakeStarts(Estrelas);
        }
        public static string MakeStarts(int estrelas)
        {
            var estrelas_string = "";
            for (int i = 0; i < estrelas; i++)
            {
                estrelas_string += "⭐";
            }
            return estrelas_string;
        }
        public static string MediaEstrelas_string(List<Feedback> feedbacks)
        {
            return MakeStarts((int)(float)Math.Round((Decimal)MediaEstrelas(feedbacks), 0, MidpointRounding.AwayFromZero));
        }
        public static float MediaEstrelas(List<Feedback> feedbacks)
        {
            var todasEstrelas = 0;
            foreach (var feedback in feedbacks)
            {
                todasEstrelas += feedback.Estrelas;
            }
            return (float)Math.Round((Decimal)(((float)todasEstrelas) / ((float)feedbacks.Count)), 2, MidpointRounding.AwayFromZero);
        }
    }
}