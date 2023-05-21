using EpicGames_KozlovSergey_ISP3120.Entities;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace EpicGames_KozlovSergey_ISP3120.Pages.MainWindow.Home
{
    /// <summary>
    /// Логика взаимодействия для EditGamePage.xaml
    /// </summary>
    public partial class EditGamePage : Page
    {
        Games game;
        byte[] mainImageData;
        public EditGamePage(Games game)
        {
            InitializeComponent();
            if (!Utils.CheckConnection())
            {
                MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            this.game = game;

            TitleTextBox.Text = game.Title;
            DescriptionTextBox.Text = game.description;
            VersionTextBox.Text = game.Version;
            PriceTextBox.Text = Convert.ToString(game.price);
            if (game.IsAvailableInStore)
            {
                DeleteGameButton.Visibility = Visibility.Visible;
            }
            if (!game.emptyLogo)
            {
                GameImage.Source = game.Logo;
                mainImageData = Server.GetGameImageByte(game.ID);
            }
        }

        private void EndEditGame_Click(object sender, RoutedEventArgs e)
        {
            // Если подключение к базе данных отсутствует
            if (!Utils.CheckConnection())
            {
                MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Utils.AddEditGameErrors error = Server.EditGame(TitleTextBox.Text,
                                           game.ID,
                                           DescriptionTextBox.Text,
                                           VersionTextBox.Text,
                                           PriceTextBox.Text,
                                           mainImageData);

            switch (error)
            {
                case Utils.AddEditGameErrors.GameTitleIsEmpty:
                    MessageBox.Show("Введите название игры",
                                "Информация",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                    break;
                case Utils.AddEditGameErrors.GameTitleIsLong:
                    MessageBox.Show("Длина названия игры не должа превышать 100 символов!",
                                "Ошибка!",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                    break;
                case Utils.AddEditGameErrors.GameVersionIsLong:
                    MessageBox.Show("Длина версии игры не должна превышать 50 символов!",
                                "Ошибка!",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                    break;
                case Utils.AddEditGameErrors.GamePriceHasLetters:
                    MessageBox.Show("Цена не должна содержать в себе буквы!",
                                    "Ошибка!",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    break;
                case Utils.AddEditGameErrors.GamePriceIsOutOfRange:
                    MessageBox.Show("Цена игры должна быть в промежутке от 0 до 10000 рублей",
                                    "Ошибка!",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    break;
                case Utils.AddEditGameErrors.NoErrors:
                    MessageBox.Show("Игра успешно отредактирована!",
                                    "Информация",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                    NavigationService.GoBack();
                    break;
            }
        }

        private void DeleteGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Если подключение к базе данных отсутствует
            if (!Utils.CheckConnection())
            {
                MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show("Вы действительно хотите удалить игру?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Server.HideGame(game.ID);
                MessageBox.Show("Игра успешно удалена!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService.Navigate(new HomePage());
                ((EpicGames_KozlovSergey_ISP3120.MainWindow)Window.GetWindow(this)).HomeTaskBarRadioButton.IsChecked = true;
            }
        }

        private void RemoveImageButton_Click(object sender, RoutedEventArgs e)
        {
            mainImageData = null;
            GameImage.Source = null;
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
                    GameImage.Source = (ImageSource)new ImageSourceConverter()
                                       .ConvertFrom(mainImageData);
                }
                // Если не удаётся преобразовать файл в картинку
                catch (NotSupportedException)
                {
                    MessageBox.Show("Не удалось установить картинку игры!",
                                    "Ошибка!",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    mainImageData = tempData;
                }
            }
        }

        private void CancelEditButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
