using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Api.Helpers;
using Api.Enities;

namespace Api.Service
{
    public interface IUserService
    {
        UserEntitis Auth(string email, string password);
        IEnumerable<Account> GetAll();
        Account GetByID(int id);
        UserEntitis Create(Account account, String password);
        void Update(Account account);
        void Delete(int id);
        public class UserEntitis
        {
            public AccountResponseModel account {get;set;}
            public String Token { get; set; }

            public UserEntitis(Account account)
            {
                this.account = new AccountResponseModel(account, false);
            }
            public UserEntitis createUserToken()
            {
                List<Claim> lstClaim = new List<Claim>(); //CLAIM USER INFO
                lstClaim.Add(new Claim(ClaimTypes.Email, this.account.Email));
                lstClaim.Add(new Claim(ClaimTypes.Role, this.account.Role.Id.ToString()));

                //CREATE JWT TOKEN
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("vclvclvclvclvclvclvclvclvclvclvclvclvclvclvclvclvclvclvclvcl");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(lstClaim.ToArray()),
                    Expires = DateTime.UtcNow.AddDays(100),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)

                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                this.Token = tokenHandler.WriteToken(token);
                return this;
            }
        }

    }
    public class UserService : IUserService
    {
        FreeLancerVNContext context = new FreeLancerVNContext();
        public IUserService.UserEntitis Auth(string email, string password)
        {
            var list = context.Accounts
                .Include(p => p.FreelancerSkills).ThenInclude(p => p.Skill)
                .AsSplitQuery()
                .Include(p => p.FreelancerServices).ThenInclude(p => p.Service)
                .AsSplitQuery()
                .Include(p => p.Specialty)
                .AsSplitQuery()
                .Include(p => p.Role)
                .AsSplitQuery()
                .Include(p => p.RatingFreelancers)
                .AsSplitQuery()
                .Include(p => p.Level)
                .AsSplitQuery()
                .Include(p => p.OfferHistories)
                .AsSplitQuery()
                .Include(p => p.CapacityProfiles)
                .ThenInclude(p => p.ProfileServices).ThenInclude(p => p.Service)
                .AsSplitQuery().ToList();
            Account account = list.SingleOrDefault(p => p.Email == email );
            if (account == null)
            {
                throw new AppException("Email doesn't exist");
            }
            if (account.BannedAtDate != null)
            {
                throw new AppException("Your account was bannish");
            }
            if (!VerifyPasswordHash(password, account.PasswordHash, account.PasswordSalt))
                throw new AppException("Password not correct");
            IUserService.UserEntitis userEntitis = new IUserService.UserEntitis(account);
            userEntitis.createUserToken();
            return userEntitis;
        }

        public IUserService.UserEntitis Create(Account account, string password)
        {
            try
            {
                MailAddress m = new MailAddress(account.Email);
            }
            catch (FormatException)
            {
                throw new AppException("Email not formatted correctly");
            }
            var acc = context.Accounts.SingleOrDefault(p => p.Email == account.Email);
            if (acc != null)
            {
                throw new AppException("Email already exist");
            }
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            account.PasswordHash = passwordHash;
            account.PasswordSalt = passwordSalt;
            account.Balance = 0;
            context.Accounts.Add(account);
            context.SaveChanges();
            account = context.Accounts
                .Include(p => p.Role).SingleOrDefault(p => p.Id == account.Id);

            IUserService.UserEntitis userEntitis = new IUserService.UserEntitis(account);
            userEntitis.createUserToken();
            return userEntitis;
        }

        public void Delete(int id)
        {
            var account = context.Accounts.Find(id);
            if (account != null)
            {
                context.Remove(account);
            }

        }

        public IEnumerable<Account> GetAll()
        {
            throw new NotImplementedException();
        }

        public Account GetByID(int id)
        {
            var account = context.Accounts.Find(id);
            if (account != null)
            {
                return account;
            }
            return null;
        }

        public void Update(Account account)
        {
            var acc = context.Accounts.Find(account.Id);
            if (acc != null)
            {
                acc = account;
                context.SaveChanges();
            }
            return;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
