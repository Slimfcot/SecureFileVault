using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SecureFileVault.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<VaultFile> VaultFiles { get; set; } = new();
        public List<AuditLog> AuditLogs { get; set; } = new();
    }
}