using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using InzynierkaAPI.Data;
using Azure.Storage.Blobs;
using Azure.Storage;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace InzynierkaAPI.Models
{
    public class Plik
    {
        [Key]
        public Guid Id { get; set; }
        public string Nazwa { get; set; }
        public string Url { get; set; }

        public virtual KategoriaPlik KategoriaPlik { get; set; }
        public int KategoriaPlikId  { get; set; }

    }
    public enum Status
    {
        Niezweryfikowany, 
        Wybrany, 
        Odrzucony,
        Wyslany,
        Anulowany,
        Realizowany,
        Przegrany,
        Zakonczony
    }

    public class Przetarg

    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100, ErrorMessage = "Maksymalna ilość znaków dla pola Lokalizacja wynosi 100.")]
        public string PrzedmiotOgloszenia { get; set; }
        public DateTime DataUtworzenia { get; set; }
        public DateTime DataPrzetargu { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(50, ErrorMessage = "Maksymalna ilość znaków dla pola Lokalizacja wynosi 50.")]
        public string Lokalizacja { get; set; }
        public virtual ICollection<Plik> Pliki { get; set; }
        public string Notatki { get; set; }

        public int Wadium { get; set; }

        public Status Status { get; set; }

        public virtual WystawcaPrzetargu WystawcaPrzetargu { get; set; }
        public int WystawcaPrzetarguId { get; set; }


    }


}
