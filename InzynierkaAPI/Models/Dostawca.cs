using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InzynierkaAPI.Models
{
    public class Dostawca
    {
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100, ErrorMessage = "Maksymalna ilość znaków dla pola Nazwa wynosi 100.")]
        public string Nazwa { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(40, ErrorMessage = "Maksymalna ilość znaków dla pola Ulica wynosi 40.")]
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

        [Required]
        [MinLength(10)]
        [MaxLength(10, ErrorMessage = "Maksymalna ilość znaków dla pola Nip wynosi 10.")]

        public string Nip { get; set; }        
        
        [Required]
        [MinLength(10)]
        [MaxLength(10, ErrorMessage = "Maksymalna ilość znaków dla pola Krs wynosi 10.")]
        public string Krs { get; set; }

    }
}
