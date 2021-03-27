using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCore.Models;

namespace WebCore.Services
{
    public interface IUserService
    {
        public UserEntities Auth(string username, string password);
    }
    public class UserService : IUserService
    {
        FreeLancerVNContext context;
        public UserService(FreeLancerVNContext context)
        {
            this.context = context;
        }
        public UserEntities Auth(string username, string password)
        {
            var user = context.Users.SingleOrDefault(p => p.Username == username && p.Password == password);
            if(user == null)
            {
                return null;
            }
            
        }
    }
}
