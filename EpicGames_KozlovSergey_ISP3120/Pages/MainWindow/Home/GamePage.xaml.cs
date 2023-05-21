using EpicGames_KozlovSergey_ISP3120.Entities;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Image = System.Windows.Controls.Image;

namespace EpicGames_KozlovSergey_ISP3120.Pages.MainWindow.Home
{
    /// <summary>
    /// Логика взаимодействия для GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        int gameID;
        Games game;
        public GamePage(int gameID)
        {
            InitializeComponent();
            this.gameID = gameID;

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Если подключение к базе данных отсутствует
            if (!Utils.CheckConnection())
            {
                MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            game = Server.GetGame(gameID);
            DataContext = game;
            SetImage(Server.IsGameInWishlist(Utils.CurrentUserLogin, game.ID));
        }

        private void SetImage(bool addGameInWishlist)
        {
            Bitmap imageBM;
            ImageSource imageSource;
            if (addGameInWishlist)
            {
                imageBM = Properties.Resources.FilledWishlistButton;
            }
            else
            {
                imageBM = Properties.Resources.NotFilledWishlistButton;
            }

            using (var ms = new MemoryStream())
            {
                imageBM.Save(ms, imageBM.RawFormat);
                imageSource = (ImageSource)new ImageSourceConverter().ConvertFrom(ms.ToArray());
            }
            Image image = new Image()
            {
                Source = imageSource,
                Margin = new Thickness(5)
            };

            WishlistButton.Content = image;
        }

        private void WishlistButton_Click(object sender, RoutedEventArgs e)
        {
            // Если подключение к базе данных отсутствует
            if (!Utils.CheckConnection())
            {
                MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Server.IsGameInWishlist(Utils.CurrentUserLogin, game.ID))
            {
                Server.RemoveFromWishlist(Utils.CurrentUserLogin, game.ID);
                SetImage(false);
            }
            else
            {
                Server.AddToWishlist(Utils.CurrentUserLogin, game.ID);
                SetImage(true);
            }

        }

        private void EditPageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EditGamePage(DataContext as Games));
        }

        private void BuyGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Если подключение к базе данных отсутствует
            if (!Utils.CheckConnection())
            {
                MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show($"Вы действительно хотите купить игру за {game.Price}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (Server.BuyGame(Utils.CurrentUserLogin, game.ID))
                {
                    Utils.vm.CurrentUser = Server.GetUser(Utils.CurrentUserLogin);
                    ((EpicGames_KozlovSergey_ISP3120.MainWindow)Window.GetWindow(this)).BalanceTextBlock.Text = Utils.vm.CurrentUser.BalanceString;
                    MessageBox.Show("Игра была успешно добавлена в вашу библиотеку!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                    BuyGameButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Недостаточно средств на балансе!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


    }
}
