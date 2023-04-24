using System.IdentityModel.Tokens.Jwt;
using System.ComponentModel.DataAnnotations;
namespace InzynierkaAPI.Models
{
    public class UserLogin
    {
        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }
}
