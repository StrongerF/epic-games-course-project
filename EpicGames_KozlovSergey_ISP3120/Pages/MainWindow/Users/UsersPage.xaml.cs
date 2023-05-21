using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace EpicGames_KozlovSergey_ISP3120.Pages.MainWindow.Users
{
    /// <summary>
    /// Логика взаимодействия для UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {
        public List<Entities.Users> users;
        public UsersPage()
        {
            InitializeComponent();

            DataContext = Utils.vm;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Если подключение к базе данных отсутствует
            if (!Utils.CheckConnection())
            {
                MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            users = Server.GetUsers(Utils.CurrentUserLogin);
            UsersListView.ItemsSource = users;
        }


        private void UserLibraryButton_Click(object sender, RoutedEventArgs e)
        {
            Entities.Users user = (sender as Button).DataContext as Entities.Users;
            NavigationService.Navigate(new Library.LibraryPage(user));
        }

        private void UserBalanceButton_Click(object sender, RoutedEventArgs e)
        {
            Entities.Users user = (sender as Button).DataContext as Entities.Users;
            NavigationService.Navigate(new BalanceReplenishmentPage(user, false));
        }

        private void UserEditButton_Click(object sender, RoutedEventArgs e)
        {
            Entities.Users user = (sender as Button).DataContext as Entities.Users;
            NavigationService.Navigate(new EditUserPage(user, false));
        }

    }
}
