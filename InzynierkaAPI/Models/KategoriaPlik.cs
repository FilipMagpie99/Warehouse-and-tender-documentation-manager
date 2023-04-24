using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Text.Json.Serialization;

namespace InzynierkaAPI.Models
{
    public class KategoriaPlik
    {
    public int Id { get; set; }

    [StringLength(30,ErrorMessage ="Maksymalna ilość znaków dla pola Nazwa wynosi 30.")]
    public string Nazwa { get; set; }

    }
}
