using System.Windows;
using SecureFileVault.Services;

namespace SecureFileVault
{
    public partial class SignUpWindow : Window
    {
        private readonly AuthService _authService = new AuthService();

        public SignUpWindow()
        {
            InitializeComponent();
        }

        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;

            bool success = _authService.Register(username, email, password, out string message);
            MessageBox.Show(message);

            if (success)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                Close();
            }
        }

        private void BackToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
    }
}