using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InzynierkaAPI.Models
{
    public enum TypOperacji
    {
        Wydanie,
        Przyjecie
    }
    public class RaportMagazynu
    {
        public int Id { get; set; }
        public DateTime DataUtworzenia { set => DataUtworzenia =  DateTime.Now;
            
        }
        public int Ilosc { get; set; }

        [Column(TypeName = "decimal")]
        [Range(0.01, 9999999999999999.99)]
        [Precision(10)]
        public decimal CenaJednostkowa { get; set; }
        public TypOperacji TypOperacji { get; set; }
        public int ProduktId { get; set; }
        public virtual User User { get; set; }
        public Guid UserId { get; set; }

        public virtual Strefa Strefa { get; set; }

        public int StrefaId { get; set; }

        //[Required(ErrorMessage = "Pole dostawca jest obowiązkowe")]
        public virtual Dostawca Dostawca { get; set; }

        //[Required(ErrorMessage ="Pole dostawca jest obowiązkowe")]
        public int? DostawcaId { get; set; }


    }
}
