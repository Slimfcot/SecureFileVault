using System.Linq;
using BCrypt.Net;
using SecureFileVault.Data;
using SecureFileVault.Models;

namespace SecureFileVault.Services
{
    public class AuthService
    {
        public bool Register(string username, string email, string password, out string message)
        {
            using var db = new AppDbContext();

            if (db.Users.Any(u => u.Email == email))
            {
                message = "An account with this email already exists.";
                return false;
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hashedPassword
            };

            db.Users.Add(user);
            db.SaveChanges();

            message = "Account created successfully.";
            return true;
        }

        public bool Login(string email, string password, out string message)
        {
            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                message = "User not found.";
                return false;
            }

            bool validPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!validPassword)
            {
                message = "Invalid password.";
                return false;
            }

            message = "Login successful.";
            return true;
        }
    }
}