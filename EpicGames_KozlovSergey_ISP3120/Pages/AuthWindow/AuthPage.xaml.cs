using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MessageBox = System.Windows.MessageBox;

namespace EpicGames_KozlovSergey_ISP3120.Pages.AuthWindow
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }



        private void RegHyperlink_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            // Если подключение к базе данных отсутствует
            if (!Utils.CheckConnection())
            {
                MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Убираются все пробелы в логине
            LoginTextBox.Text = LoginTextBox.Text.Replace(" ", "").ToLower();

            // Создание переменной, которая при вызове метода Server.Auth() возвращает одно из перечислений LoginErrors
            Utils.LoginErrors error = Server.Auth(LoginTextBox.Text, PasswordTextBox.Password);
            if (error == Utils.LoginErrors.LoginIsEmpty)
            {
                MessageBox.Show("Введите логин", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (error == Utils.LoginErrors.PasswordIsEmpty)
            {
                MessageBox.Show("Введите пароль", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (error == Utils.LoginErrors.UserDoesNotExist)
            {
                MessageBox.Show("Такого пользователя не существует!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (error == Utils.LoginErrors.IncorrectPassword)
            {
                MessageBox.Show("Вы ввели неправильный пароль!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (error == Utils.LoginErrors.AccountIsDeactivated)
            {
                MessageBox.Show("Данный аккаунт деактивирован. Для активации необходимо написать письмо на электронную почту: sergey5942020@gmail.com", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (error == Utils.LoginErrors.NoErrors)
            {
                // Присваивание логина войденного пользователя
                Utils.CurrentUserLogin = LoginTextBox.Text;
                // Выполнение метода удачного входа из окна
                // Получение формы, в которой находится страница и преобразование в унаследованную форму AuthWindow и обращение к его методу SuccessLogin
                ((EpicGames_KozlovSergey_ISP3120.AuthWindow)Window.GetWindow(this)).SuccessLogin();
            }
        }
    }
}
