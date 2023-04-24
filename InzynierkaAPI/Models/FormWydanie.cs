using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InzynierkaAPI.Models
{
    public class FormWydanie
    {
        [Range(1, int.MaxValue, ErrorMessage = "Nie możesz wydać lub przyjąć mniej niż 1 produktu")]

        public int Ilosc { get; set; }

    }
}
