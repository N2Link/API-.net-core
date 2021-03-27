using WebCore.Models;

namespace WebCore.Services
{
    public class UserEntities
    {
        public User user;
        public string Token { get; set; }
        public UserEntities(User user)
        {
            this.user = user;

        }
    }
}