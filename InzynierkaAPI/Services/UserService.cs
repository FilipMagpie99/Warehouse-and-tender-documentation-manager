using InzynierkaAPI.Models;


namespace InzynierkaAPI.Services
{
    public class UserService : IUserService
    {
        public User Get(UserLogin userLogin, DataContext db)
        {
            User user = db.User.FirstOrDefault(o => o.Username.Equals
            (userLogin.Username) && o.Password.Equals
            (userLogin.Password));

            return user;
        }
    }
}
