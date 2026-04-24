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
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            var user = _authService.Login(email, password, out string message);

            if (user == null)
            {
                MessageBox.Show(message);
                return;
            }

            MessageBox.Show(message);

            var dashboard = new DashboardWindow(user);
            dashboard.Show();
            Close();
        }

        private void OpenSignUpButton_Click(object sender, RoutedEventArgs e)
        {
            var signUpWindow = new SignUpWindow();
            signUpWindow.Show();
            Close();
        }
    }
}
