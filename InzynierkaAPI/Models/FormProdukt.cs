using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InzynierkaAPI.Models
{
    public class FormProdukt
    {
        [Range(1, int.MaxValue,ErrorMessage="Nie możesz wydać lub przyjąć mniej niż 1 produktu")]

        public int Ilosc { get; set; }

        [Column(TypeName = "decimal")]
        [Range(0.01, 9999999999999999.99,ErrorMessage ="Cena nie może być mniejsza niz 1 grosz")]
        [Precision(10)]
        public decimal CenaJednostkowa { get; set; }
    }
}
