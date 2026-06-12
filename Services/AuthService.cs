using System.Linq;
using SecureFileVault.Data;
using SecureFileVault.Models;

namespace SecureFileVault.Services
{
    public class AuthService
    {
        public bool Register(string username, string email, string password, out string message)
        {
            username = username.Trim();
            email = email.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                message = "All fields are required.";
                return false;
            }

            using var db = new AppDbContext();

            if (db.Users.Any(u => u.Email == email))
            {
                message = "An account with this email already exists.";
                return false;
            }

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            db.Users.Add(user);
            db.SaveChanges();

            message = "Account created successfully.";
            return true;
        }

        public User? Login(string email, string password, out string message)
        {
            email = email.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                message = "Email and password are required.";
                return null;
            }

            using var db = new AppDbContext();

            var user = db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                message = "User not found.";
                return null;
            }

            bool valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!valid)
            {
                message = "Invalid password.";
                return null;
            }

            db.AuditLogs.Add(new AuditLog
            {
                UserId = user.UserId,
                ActionType = "Login",
                Details = "User logged in",
                Timestamp = DateTime.UtcNow
            });

            db.SaveChanges();

            message = "Login successful.";
            return user;
        }
    }
}