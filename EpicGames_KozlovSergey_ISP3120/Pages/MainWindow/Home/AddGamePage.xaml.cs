using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace EpicGames_KozlovSergey_ISP3120.Pages.MainWindow.Home
{
    /// <summary>
    /// Логика взаимодействия для AddGamePage.xaml
    /// </summary>
    public partial class AddGamePage : Page
    {
        byte[] mainImageData;


        public AddGamePage()
        {
            InitializeComponent();
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

        private void PublishGame_Click(object sender, RoutedEventArgs e)
        {
            // Если подключение к базе данных отсутствует
            if (!Utils.CheckConnection())
            {
                MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Utils.AddEditGameErrors error = Server.AddGame(TitleTextBox.Text,
                                            Utils.CurrentUserLogin,
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
                    MessageBox.Show("Игра успешно опубликована!",
                                    "Информация",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                    mainImageData = null;
                    GameImage.Source = null;
                    TitleTextBox.Text = string.Empty;
                    DescriptionTextBox.Text = string.Empty;
                    VersionTextBox.Text = string.Empty;
                    PriceTextBox.Text = string.Empty;
                    break;
            }
        }


        private void RemoveImageButton_Click(object sender, RoutedEventArgs e)
        {
            mainImageData = null;
            GameImage.Source = null;
        }
    }
}
