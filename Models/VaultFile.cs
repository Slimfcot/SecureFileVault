using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureFileVault.Models
{
    public class VaultFile
    {
        [Key]
        public int FileId { get; set; }

        [Required]
        public int OwnerUserId { get; set; }

        [ForeignKey(nameof(OwnerUserId))]
        public User? OwnerUser { get; set; }

        [Required]
        [MaxLength(255)]
        public string OriginalFileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string StoredFileName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string FileExtension { get; set; } = string.Empty;

        public long FileSize { get; set; }

        [Required]
        [MaxLength(500)]
        public string EncryptedPath { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        [NotMapped]
        public string FileSizeMb
        {
            get
            {
                return $"{FileSize / 1024.0 / 1024.0:F2}";
            }
        }
            }
}