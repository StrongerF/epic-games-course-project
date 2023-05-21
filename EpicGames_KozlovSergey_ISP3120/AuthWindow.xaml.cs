using System.Windows;

namespace EpicGames_KozlovSergey_ISP3120
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
            AuthFrame.Navigate(new Pages.AuthWindow.AuthPage());
        }

        public void SuccessLogin()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Owner = this;
            Hide();
            mainWindow.Show();
        }
    }
}
