using System;
using System.IO;
using System.Linq;
using SecureFileVault.Data;
using SecureFileVault.Models;

namespace SecureFileVault.Services
{
    public class VaultFileService
    {
        private readonly string _storageFolder;

        public VaultFileService()
        {
            _storageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VaultStorage");

            if (!Directory.Exists(_storageFolder))
            {
                Directory.CreateDirectory(_storageFolder);
            }
        }

        public bool UploadFile(int ownerUserId, string sourceFilePath, out string message)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath) || !File.Exists(sourceFilePath))
            {
                message = "Selected file does not exist.";
                return false;
            }

            try
            {
                string originalFileName = Path.GetFileName(sourceFilePath);
                string extension = Path.GetExtension(sourceFilePath);
                long fileSize = new FileInfo(sourceFilePath).Length;

                string storedFileName = $"{Guid.NewGuid()}{extension}";
                string destinationPath = Path.Combine(_storageFolder, storedFileName);

                File.Copy(sourceFilePath, destinationPath, overwrite: false);

                using var db = new AppDbContext();

                var vaultFile = new VaultFile
                {
                    OwnerUserId = ownerUserId,
                    OriginalFileName = originalFileName,
                    StoredFileName = storedFileName,
                    FileExtension = extension,
                    FileSize = fileSize,
                    EncryptedPath = destinationPath,
                    UploadedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                db.VaultFiles.Add(vaultFile);

                db.AuditLogs.Add(new AuditLog
                {
                    UserId = ownerUserId,
                    FileId = null,
                    ActionType = "Upload",
                    Details = $"Uploaded file: {originalFileName}",
                    Timestamp = DateTime.UtcNow
                });

                db.SaveChanges();

                message = "File encrypted and securely stored.";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Upload failed: {ex.Message}";
                return false;
            }
        }

        public IQueryable<VaultFile> GetFilesForUser(int ownerUserId)
        {
            var db = new AppDbContext();
            return db.VaultFiles.Where(f => f.OwnerUserId == ownerUserId && !f.IsDeleted);
        }
    }
}