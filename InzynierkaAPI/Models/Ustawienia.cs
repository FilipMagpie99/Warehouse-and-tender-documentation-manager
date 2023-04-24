using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InzynierkaAPI.Models
{
    public class Ustawienia
    {
        [Key]
        public string Id { get; set; }
        public bool Wartosc { get; set; }

    }
}
