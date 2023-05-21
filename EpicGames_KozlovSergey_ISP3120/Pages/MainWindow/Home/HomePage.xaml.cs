using EpicGames_KozlovSergey_ISP3120.Entities;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace EpicGames_KozlovSergey_ISP3120.Pages.MainWindow.Home
{
    /// <summary>
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public int[] userWishlist;
        public List<Games> games;
        public HomePage()
        {
            InitializeComponent();

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Если подключение к базе данных отсутствует
            if (!Utils.CheckConnection())
            {
                MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            games = Server.GetGames();
            userWishlist = Server.GetUserWishlist(Utils.vm.CurrentUser.ID);
            GamesListView.ItemsSource = games;
        }


        private void GameButton_Click(object sender, RoutedEventArgs e)
        {
            Games currentGame = (sender as Button).DataContext as Games;
            NavigationService.Navigate(new GamePage(currentGame.ID));
        }

    }
}
