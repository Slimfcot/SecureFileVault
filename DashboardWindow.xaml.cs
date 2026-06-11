using Microsoft.Win32;
using System.Linq;
using System.IO;
using System.Windows;
using SecureFileVault.Models;
using SecureFileVault.Services;

namespace SecureFileVault
{
    public partial class DashboardWindow : Window
    {
        private readonly User _currentUser;
        private readonly VaultFileService _vaultFileService = new VaultFileService();

        public DashboardWindow(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            UsernameTextBlock.Text = _currentUser.Username;
            LoadFiles();
        }

        private void LoadFiles()
        {
            var files = _vaultFileService
                .GetFilesForUser(_currentUser.UserId)
                .ToList();

            FilesDataGrid.ItemsSource = files;
        }

        private void UploadFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select a file to upload"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                bool success = _vaultFileService.UploadFile(
                    _currentUser.UserId,
                    openFileDialog.FileName,
                    out string message);

                MessageBox.Show(message, "Upload", MessageBoxButton.OK,
                    success ? MessageBoxImage.Information : MessageBoxImage.Error);

                if (success)
                {
                    LoadFiles();
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadFiles();
        }

        private void DownloadFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilesDataGrid.SelectedItem is not VaultFile selectedFile)
            {
                MessageBox.Show("Please select a file.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var saveDialog = new SaveFileDialog
            {
                FileName = selectedFile.OriginalFileName
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    var encryptionService = new EncryptionService();

                    byte[] key = encryptionService.GenerateKey("demo-key");
                    byte[] iv = encryptionService.GenerateIV();

                    encryptionService.DecryptFile(
                    selectedFile.EncryptedPath,
                    saveDialog.FileName,
                    key,
                    iv);
                    MessageBox.Show("File downloaded successfully.", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Download failed: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilesDataGrid.SelectedItem is not VaultFile selectedFile)
            {
                MessageBox.Show("Please select a file.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                "Are you sure you want to delete this file?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes)
                return;

            using var db = new Data.AppDbContext();

            var file = db.VaultFiles.FirstOrDefault(f => f.FileId == selectedFile.FileId);

            if (file != null)
            {
                file.IsDeleted = true;

                db.AuditLogs.Add(new AuditLog
                {
                    UserId = _currentUser.UserId,
                    FileId = file.FileId,
                    ActionType = "Delete",
                    Details = $"Deleted file: {file.OriginalFileName}"
                });

                db.SaveChanges();
            }

             LoadFiles();
        }

            private void AuditLogsButton_Click(object sender, RoutedEventArgs e)
        {
            var logsWindow = new AuditLogsWindow();
            logsWindow.ShowDialog();
        }
        
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
    }
}
