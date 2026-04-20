using System.Windows;
using SecureFileVault.Services;

namespace SecureFileVault
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService = new AuthService();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;

            bool success = _authService.Login(email, password, out string message);
            MessageBox.Show(message);

            if (success)
            {
                MessageBox.Show("Dashboard will be added next.");
            }
        }

        private void OpenSignUpButton_Click(object sender, RoutedEventArgs e)
        {
            var signUpWindow = new SignUpWindow();
            signUpWindow.Show();
            Close();
        }
    }
}