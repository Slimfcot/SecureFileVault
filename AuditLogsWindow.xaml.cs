using System.Linq;
using System.Windows;
using SecureFileVault.Data;

namespace SecureFileVault
{
    public partial class AuditLogsWindow : Window
    {
        public AuditLogsWindow()
        {
            InitializeComponent();

            using var db = new AppDbContext();

            AuditLogsGrid.ItemsSource =
                db.AuditLogs
                .OrderByDescending(x => x.Timestamp)
                .ToList();
        }
    }
}