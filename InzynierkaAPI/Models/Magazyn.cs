using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Text.Json.Serialization;

namespace InzynierkaAPI.Models
{
    public class Magazyn
    {
		public int Id { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(30, ErrorMessage = "Maksymalna ilość znaków dla pola Nazwa wynosi 30.")]
        public string Nazwa { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(20, ErrorMessage = "Maksymalna ilość znaków dla pola Ulica wynosi 20.")]
        public string Ulica { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(30, ErrorMessage = "Maksymalna ilość znaków dla pola Numer domu wynosi 30.")]
        public string NrDomu { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(6, ErrorMessage = "Maksymalna ilość znaków dla pola kod pocztowy wynosi 6.")]
        public string KodPocztowy { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(30, ErrorMessage = "Maksymalna ilość znaków dla pola Miejscowość wynosi 30.")]
        public string Miejscowosc { get; set; }
        public virtual ICollection<Strefa> Strefa { get; set; }
    



    }
}
