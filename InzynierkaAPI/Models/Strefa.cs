using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Mozilla;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Text.Json.Serialization;

namespace InzynierkaAPI.Models
{
    public class Strefa
    {
    public int Id { get; set; }
        [Required]
        [MinLength(1)]
        [MaxLength(20, ErrorMessage = "Maksymalna ilość znaków dla pola Nazwa wynosi 20.")]
        public string Nazwa { get; set; }
    public virtual ICollection<Produkt> Produkt { get; set; }

/*    [JsonIgnore]
    public virtual Magazyn Magazyn { get; set; }

    public int MagazynId { get; set; }*/

    }
}
