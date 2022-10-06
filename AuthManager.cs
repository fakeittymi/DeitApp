using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DeitApp
{
    class AuthManager
    {
        public bool Authorize(string login, string password)
        {
            using (BalancedDietEntities db = new BalancedDietEntities())
            {

                UsersAuth auth = db.UsersAuth.Where(e => e.Login == login).FirstOrDefault();
                if (auth == null)
                {
                    return false;
                }

                if (auth.Password == GetMD5Hash(password))
                {
                    currentUserId = auth.UserId;
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public bool Register(string login, string password)
        {
            using (BalancedDietEntities db = new BalancedDietEntities())
            {
                if (db.UsersAuth.Where(e => e.Login == login).FirstOrDefault() != null)
                    return false;

                UsersAuth newUser = new UsersAuth();
                newUser.id = Guid.NewGuid();
                newUser.UserId = Guid.NewGuid();
                newUser.Login = login;
                newUser.Password = GetMD5Hash(password);
                db.UsersAuth.Add(newUser);
                db.SaveChanges();
                return true;
            }
        }

        public Guid GetUserId()
        {
            return currentUserId;
        }

        private string GetMD5Hash(string text)
        {
            using (var hashAlg = MD5.Create())
            {
                byte[] hash = hashAlg.ComputeHash(Encoding.UTF8.GetBytes(text));
                var builder = new StringBuilder(hash.Length * 2);
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("X2"));
                }
                return builder.ToString();
            }
        }

        Guid currentUserId;
    }
}
