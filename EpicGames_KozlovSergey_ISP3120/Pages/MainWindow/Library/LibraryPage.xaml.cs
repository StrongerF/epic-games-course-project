using EpicGames_KozlovSergey_ISP3120.Entities;
using EpicGames_KozlovSergey_ISP3120.Pages.MainWindow.Home;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace EpicGames_KozlovSergey_ISP3120.Pages.MainWindow.Library
{
    /// <summary>
    /// Логика взаимодействия для LibraryPage.xaml
    /// </summary>
    public partial class LibraryPage : Page
    {
        public int[] userWishlist;
        public Entities.Users user;
        public List<Games> games;
        public LibraryPage(Entities.Users user)
        {
            InitializeComponent();

            this.user = user;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Если подключение к базе данных отсутствует
            if (!Utils.CheckConnection())
            {
                MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            games = Server.GetUserLibrary(user.ID);
            userWishlist = Server.GetUserWishlist(user.ID);
            GamesListView.ItemsSource = games;
        }

        private void GameButton_Click(object sender, RoutedEventArgs e)
        {
            Games currentGame = (sender as Button).DataContext as Games;
            NavigationService.Navigate(new GamePage(currentGame.ID));
        }

        private void DeleteGameFromLibraryButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить игру из библиотеки?\nДенежные средства, потраченные на игру, не будут возвращены!", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // Если подключение к базе данных отсутствует
                if (!Utils.CheckConnection())
                {
                    MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Games currentGame = (sender as Button).DataContext as Games;

                if (Server.DeleteGameFromLibrary(user.ID, currentGame.ID))
                {
                    games = Server.GetUserLibrary(user.ID);
                    GamesListView.ItemsSource = games;
                    ((EpicGames_KozlovSergey_ISP3120.MainWindow)Window.GetWindow(this)).UpdateGames();
                    MessageBox.Show("Игра была успешно удалена из библиотеки!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось удалить игру из библиотеки!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
    }
}
