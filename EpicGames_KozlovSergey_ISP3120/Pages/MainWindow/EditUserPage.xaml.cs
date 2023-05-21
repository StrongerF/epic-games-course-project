using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace EpicGames_KozlovSergey_ISP3120.Pages.MainWindow
{
    /// <summary>
    /// Логика взаимодействия для UserEditPage.xaml
    /// </summary>
    public partial class EditUserPage : Page
    {
        public static bool IsFromMainWindow { get; set; }
        Entities.Users user;
        byte[] mainImageData;
        bool isCurrentUser;
        public EditUserPage(Entities.Users user, bool isCurrentUser)
        {
            InitializeComponent();
            DataContext = user;
            // Если подключение к базе данных отсутствует
            if (!Utils.CheckConnection())
            {
                MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            this.user = user;
            this.isCurrentUser = isCurrentUser;

            NicknameTextBox.Text = user.Nickname;
            LoginTextBlock.Text = user.Login;

            if (!user.emptyAvatar)
            {
                UserImage.Source = user.Avatar;
                mainImageData = Server.GetUserImageData(user.ID);
            }

            IsFromMainWindow = isCurrentUser;
        }




        private void RemoveImageButton_Click(object sender, RoutedEventArgs e)
        {
            mainImageData = null;
            UserImage.Source = null;
        }

        private void ChooseImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Image (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                byte[] tempData = mainImageData;
                mainImageData = File.ReadAllBytes(openFileDialog.FileName);
                try
                {
                    UserImage.Source = (ImageSource)new ImageSourceConverter()
                                       .ConvertFrom(mainImageData);
                }
                // Если не удаётся преобразовать файл в картинку
                catch (NotSupportedException)
                {
                    MessageBox.Show("Не удалось установить картинку пользователя!",
                                    "Ошибка!",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    mainImageData = tempData;
                }
            }
        }

        private void EndEditUser_Click(object sender, RoutedEventArgs e)
        {
            // Если подключение к базе данных отсутствует
            if (!Utils.CheckConnection())
            {
                MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            NicknameTextBox.Text = NicknameTextBox.Text.Trim().Replace("  ", " ");
            Utils.AddEditUserErrors error = Server.EditUser(NicknameTextBox.Text,
                                                            user.ID,
                                                            mainImageData,
                                                            OldPasswordTextBox.Password,
                                                            NewPasswordTextBox.Password,
                                                            NewSecondPasswordTextBox.Password);

            if (error == Utils.AddEditUserErrors.UserNicknameIsEmpty)
            {
                MessageBox.Show("Введите никнейм", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (error == Utils.AddEditUserErrors.UserNicknameIsShortOrLong)
            {
                MessageBox.Show("Никнейм должен содержать от 3 до 32 символов!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (error == Utils.AddEditUserErrors.PasswordIsEmpty)
            {
                MessageBox.Show("Введите пароль", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (error == Utils.AddEditUserErrors.PasswordIsTooShortOrLong)
            {
                MessageBox.Show("Пароль должен содержать от 8 до 32 символов!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (error == Utils.AddEditUserErrors.PasswordMustHaveDifferentCharacters)
            {
                MessageBox.Show("Пароль должен содержать в себе хотя бы одну строчную букву, одну прописную букву, одну цифру и любой другой символ", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (error == Utils.AddEditUserErrors.SecondPasswordIsEmpty)
            {
                MessageBox.Show("Повторите пароль", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (error == Utils.AddEditUserErrors.PasswordDoesNotMatch)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (error == Utils.AddEditUserErrors.OldPasswordDoesNotMatch)
            {
                MessageBox.Show("Неверно введён старый пароль!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (isCurrentUser)
                {
                    Utils.vm.CurrentUser = Server.GetUser(user.Login);
                    ((EpicGames_KozlovSergey_ISP3120.MainWindow)Window.GetWindow(this)).NicknameTextBlock.Text = Utils.vm.CurrentUser.Nickname;
                    ((EpicGames_KozlovSergey_ISP3120.MainWindow)Window.GetWindow(this)).ImageEllipse.ImageSource = Utils.vm.CurrentUser.Avatar;
                }
                if (error == Utils.AddEditUserErrors.NoErrors)
                {
                    OldPasswordTextBox.Clear();
                    NewPasswordTextBox.Clear();
                    NewSecondPasswordTextBox.Clear();
                }
                MessageBox.Show("Аккаунт был успешно отредактирован!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeactivateUserButton_Click(object sender, RoutedEventArgs e)
        {
            // Если подключение к базе данных отсутствует
            if (!Utils.CheckConnection())
            {
                MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show("Вы действительно хотите деактивировать аккаунт? Для повторной активации необходимо написать письмо на электронную почту: sergey5942020@gmail.com", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Server.DeactivateUser(user.ID);
                MessageBox.Show("Пользователь успешно деактивирован!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                if (isCurrentUser)
                {
                    ((EpicGames_KozlovSergey_ISP3120.MainWindow)Window.GetWindow(this)).Close();
                }
                else
                {
                    NavigationService.GoBack();
                }
            }
        }
        private void ActivateUserButton_Click(object sender, RoutedEventArgs e)
        {
            // Если подключение к базе данных отсутствует
            if (!Utils.CheckConnection())
            {
                MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show("Вы действительно хотите активировать аккаунт?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Server.ActivateUser(user.ID);
                MessageBox.Show("Пользователь успешно активирован!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
        }

        private void CancelEditButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }


    }
}
