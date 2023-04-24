using InzynierkaAPI.Models;

namespace InzynierkaAPI.Services
{
    public interface IUserService
    {
        public User Get(UserLogin userLogin, DataContext db);
    }
}
