using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MessageBox = System.Windows.MessageBox;

namespace EpicGames_KozlovSergey_ISP3120.Pages.AuthWindow
{
    /// <summary>
    /// Логика взаимодействия для RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
        }

        private void LoginHyperlink_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            // Если подключение к базе данных отсутствует
            if (!Utils.CheckConnection())
            {
                MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Убираются все пробелы в логине
            LoginTextBox.Text = LoginTextBox.Text.ToLower();

            // Создание переменной, которая при вызове метода Server.Registration() возвращает одно из перечислений AddEditUserErrors
            // Последним параметром указывается то, что вызов метода не является тестом
            // т.е. после вызова метода надо обязательно внести данные в базу, если ошибок не возникнет
            Utils.AddEditUserErrors error = Server.Registration(LoginTextBox.Text, PasswordTextBox.Password, SecondPasswordTextBox.Password, false);

            // Если логин не введён
            if (error == Utils.AddEditUserErrors.LoginIsEmpty)
            {
                MessageBox.Show("Введите логин", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            // Если в логине присутствуют символы, не перечисленные в переменной pattern
            else if (error == Utils.AddEditUserErrors.LoginMustHaveOtherCharacters)
            {
                MessageBox.Show("Логин может содержать в себе только латинские буквы, цифры, нижние подчёркивания и точки.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Если логин больше 20 или меньше 3 символов
            else if (error == Utils.AddEditUserErrors.LoginIsTooShortOrLong)
            {
                MessageBox.Show("Логин должен содержать от 3 до 20 символов!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Если пароль не введён
            else if (error == Utils.AddEditUserErrors.PasswordIsEmpty)
            {
                MessageBox.Show("Введите пароль", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            // Если пароль меньше 8 или больше 32 символов
            else if (error == Utils.AddEditUserErrors.PasswordIsTooShortOrLong)
            {
                MessageBox.Show("Пароль должен содержать от 8 до 32 символов!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Если в пароле отсутствуют определённые группы символов
            else if (error == Utils.AddEditUserErrors.PasswordMustHaveDifferentCharacters)
            {
                MessageBox.Show("Пароль должен содержать в себе хотя бы одну строчную букву, одну прописную букву, одну цифру и любой другой символ", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Если пароль не введён второй раз
            else if (error == Utils.AddEditUserErrors.SecondPasswordIsEmpty)
            {
                MessageBox.Show("Повторите пароль", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            // Если пароли не совпадают
            else if (error == Utils.AddEditUserErrors.PasswordDoesNotMatch)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (error == Utils.AddEditUserErrors.UserExists)
            {
                MessageBox.Show("Такой логин уже существует", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            // Иначе происходит регистрация
            else if (error == Utils.AddEditUserErrors.NoErrors)
            {
                MessageBox.Show("Вы успешно зарегистрировались!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                // Возврат на прошлую страницу (страница входа)
                NavigationService.GoBack();
            }
        }


    }
}
