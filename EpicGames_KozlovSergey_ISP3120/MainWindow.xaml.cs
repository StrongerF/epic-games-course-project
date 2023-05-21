using EpicGames_KozlovSergey_ISP3120.Pages.MainWindow;
using EpicGames_KozlovSergey_ISP3120.Pages.MainWindow.Home;
using EpicGames_KozlovSergey_ISP3120.Pages.MainWindow.Library;
using EpicGames_KozlovSergey_ISP3120.Pages.MainWindow.Users;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Image = System.Windows.Controls.Image;
using MessageBox = System.Windows.MessageBox;

namespace EpicGames_KozlovSergey_ISP3120
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool wishlistBool = false;
        public MainWindow()
        {
            InitializeComponent();
            // Если подключение к базе данных отсутствует
            if (!Utils.CheckConnection())
            {
                MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ContentFrame.Navigate(new HomePage());
            Utils.vm = new VM
            {
                CurrentUser = Server.GetUser(Utils.CurrentUserLogin)
            };
            DataContext = Utils.vm;




        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is HomePage || e.Content is LibraryPage || e.Content is UsersPage)
            {
                SearchPanel.Visibility = Visibility.Visible;
                wishlistBool = false;
                SearchTextBox.Text = string.Empty;
            }
            else
            {
                SearchPanel.Visibility = Visibility.Collapsed;
            }

            if (e.Content is HomePage || e.Content is LibraryPage)
            {
                SetWishlistImage(wishlistBool);
                WishlistButton.Visibility = Visibility.Visible;
            }
            else
            {
                WishlistButton.Visibility = Visibility.Collapsed;
            }





            if (e.Content is HomePage ||
               (e.Content is LibraryPage && LibraryTaskBarRadioButton.IsChecked == true) ||
                e.Content is UsersPage ||
                e.Content is AddGamePage ||
               (e.Content is EditUserPage && EditUserPage.IsFromMainWindow) ||
               (e.Content is BalanceReplenishmentPage && BalanceReplenishmentPage.IsFromMainWindow))
            {
                if (ContentFrame.CanGoBack)
                {
                    ContentFrame.RemoveBackEntry();
                }
                GoBackTaskBarButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                GoBackTaskBarButton.Visibility = Visibility.Visible;
            }
        }

        private void GoBackTaskBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
            }
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            Owner.Close();
        }

        private void AddGameTaskBarRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new AddGamePage());
        }

        private void HomeTaskBarRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new HomePage());
        }

        private void LibraryTaskBarRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new LibraryPage(Utils.vm.CurrentUser));
        }

        private void UsersTaskBarRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new UsersPage());
        }

        private void EditBalanceTaskBarRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new BalanceReplenishmentPage(Utils.vm.CurrentUser, true));
        }


        private void EditCurrentUserButton_Click(object sender, RoutedEventArgs e)
        {
            HomeTaskBarRadioButton.IsChecked = false;
            LibraryTaskBarRadioButton.IsChecked = false;
            UsersTaskBarRadioButton.IsChecked = false;
            AddGameTaskBarRadioButton.IsChecked = false;
            ContentFrame.Navigate(new EditUserPage(Utils.vm.CurrentUser, true));
        }



        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateGames();
        }






        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Cursor = System.Windows.Input.Cursors.Hand;
        }

        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void WishlistButton_Click(object sender, RoutedEventArgs e)
        {
            wishlistBool = !wishlistBool;
            SetWishlistImage(wishlistBool);
            UpdateGames();
        }

        void SetWishlistImage(bool isFilled)
        {
            Bitmap imageBM;
            ImageSource imageSource;
            if (isFilled)
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

        public void UpdateGames()
        {
            Page currentPage = ContentFrame.Content as Page;
            if (currentPage is HomePage)
            {
                HomePage homePageContent = (HomePage)ContentFrame.Content;
                if (wishlistBool && SearchTextBox.Text == string.Empty)
                {
                    homePageContent.GamesListView.ItemsSource = homePageContent.games.Where(g => homePageContent.userWishlist.Contains(g.ID)).ToList();
                }
                else if (SearchTextBox.Text == string.Empty)
                {
                    homePageContent.GamesListView.ItemsSource = homePageContent.games;
                }
                else if (!wishlistBool)
                {
                    homePageContent.GamesListView.ItemsSource = homePageContent.games.Where(g => g.Title.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList();
                }
                else
                {
                    homePageContent.GamesListView.ItemsSource = homePageContent.games.Where(g => homePageContent.userWishlist.Contains(g.ID) && g.Title.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList();
                }
            }
            else if (currentPage is LibraryPage)
            {
                LibraryPage libraryPageContent = (LibraryPage)ContentFrame.Content;
                if (wishlistBool && SearchTextBox.Text == string.Empty)
                {
                    libraryPageContent.GamesListView.ItemsSource = libraryPageContent.games.Where(g => libraryPageContent.userWishlist.Contains(g.ID)).ToList();
                }
                else if (SearchTextBox.Text == string.Empty)
                {
                    libraryPageContent.GamesListView.ItemsSource = libraryPageContent.games;
                }
                else if (!wishlistBool)
                {
                    libraryPageContent.GamesListView.ItemsSource = libraryPageContent.games.Where(g => g.Title.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList();
                }
                else
                {
                    libraryPageContent.GamesListView.ItemsSource = libraryPageContent.games.Where(g => libraryPageContent.userWishlist.Contains(g.ID) && g.Title.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList();
                }
            }
            else if (currentPage is UsersPage)
            {
                UsersPage usersPageContent = (UsersPage)ContentFrame.Content;
                if (SearchTextBox.Text == string.Empty)
                {
                    usersPageContent.UsersListView.ItemsSource = usersPageContent.users;
                }
                else
                {
                    usersPageContent.UsersListView.ItemsSource = usersPageContent.users.Where(u => u.Login.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList();
                }
            }
        }
    }
}
