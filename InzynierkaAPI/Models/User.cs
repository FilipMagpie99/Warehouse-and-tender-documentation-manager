using System.IdentityModel.Tokens.Jwt;
using System.ComponentModel.DataAnnotations;
namespace InzynierkaAPI.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(16, ErrorMessage = "Maksymalna ilość znaków dla pola Login wynosi 16.")]
        public string Username { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(16, ErrorMessage = "Maksymalna ilość znaków dla pola Email wynosi 16.")]
        public string EmailAdress { get; set; }
        [Required]
        [MaxLength(16, ErrorMessage = "Maksymalna ilość znaków dla pola Hasło wynosi 16.")]
        public string Password { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(16, ErrorMessage = "Maksymalna ilość znaków dla pola Imię wynosi 16.")]
        public string GivenName { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(16, ErrorMessage = "Maksymalna ilość znaków dla pola Nazwisko wynosi 16.")]
        public string Surname { get; set; }

        public string Role {get; set;}

    }

}
