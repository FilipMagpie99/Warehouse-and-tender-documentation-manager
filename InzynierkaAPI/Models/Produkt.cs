using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;

namespace InzynierkaAPI.Models
{
    public class Produkt
    {
        public Produkt()
        {
            RaportMagazynu = new HashSet<RaportMagazynu>();
        }
        public int Id { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(80, ErrorMessage = "Maksymalna ilość znaków dla pola Nazwa wynosi 80.")]
        public string Nazwa { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(30, ErrorMessage = "Maksymalna ilość znaków dla pola Typ produktu wynosi 30.")]
        public string TypProduktu { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(30, ErrorMessage = "Maksymalna ilość znaków dla pola Jednostka miary wynosi 30.")]
        public string JednostkaMiary { get; set; }
        public int Suma => RaportMagazynu.Where(x => x.TypOperacji == TypOperacji.Przyjecie).Select(x => x.Ilosc).DefaultIfEmpty(0).Sum() - RaportMagazynu.Where(x => x.TypOperacji == TypOperacji.Wydanie).Select(x => x.Ilosc).DefaultIfEmpty(0).Sum();
        public virtual Producent Producent { get; set; }

        public virtual ICollection<RaportMagazynu> RaportMagazynu { get; set; }
        public virtual ICollection<Strefa> Strefa { get; set; }
        public int ProducentId { get; set; }


    }
}
