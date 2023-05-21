using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace EpicGames_KozlovSergey_ISP3120.Pages.MainWindow.Users
{
    /// <summary>
    /// Логика взаимодействия для BalanceReplenishmentPage.xaml
    /// </summary>
    public partial class BalanceReplenishmentPage : Page
    {
        public static bool IsFromMainWindow { get; set; }
        Entities.Users user;
        public bool isCurrentUser;

        public BalanceReplenishmentPage(Entities.Users user, bool isCurrentUser)
        {
            InitializeComponent();

            this.user = user;
            this.isCurrentUser = isCurrentUser;

            IsFromMainWindow = isCurrentUser;
        }

        private void CancelEditButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ApplyEditButton_Click(object sender, RoutedEventArgs e)
        {
            // Если подключение к базе данных отсутствует
            if (!Utils.CheckConnection())
            {
                MessageBox.Show(Utils.dbErrorMessage, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Utils.EditBalanceErrors errors;

            if (ReplenishmentRadioButton.IsChecked == true)
            {
                errors = Server.EditUserBalance(user.ID, BalanceTextBox.Text, Utils.EditBalanceMethod.Replenishment);

            }
            else if (RebalanceRadioButton.IsChecked == true)
            {
                errors = Server.EditUserBalance(user.ID, BalanceTextBox.Text, Utils.EditBalanceMethod.Rebalance);
            }
            else return;

            switch (errors)
            {
                case Utils.EditBalanceErrors.BalanceTextIsEmpty:
                    MessageBox.Show("Введите значение в поле",
                                    "Информация",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                    break;

                case Utils.EditBalanceErrors.BalanceTextHasLetters:
                    MessageBox.Show("Баланс не должен содержать в себе буквы!",
                                    "Ошибка!",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    break;

                case Utils.EditBalanceErrors.BalanceNumberIsOutOfRange:
                    MessageBox.Show("Баланс должен быть в промежутке от 0 до 100000 рублей",
                                    "Ошибка!",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    break;

                case Utils.EditBalanceErrors.NoErrors:
                    if (isCurrentUser)
                    {
                        Utils.vm.CurrentUser.Balance = Server.GetUserBalance(user.ID);
                        ((EpicGames_KozlovSergey_ISP3120.MainWindow)Window.GetWindow(this)).BalanceTextBlock.Text = Utils.vm.CurrentUser.BalanceString;
                    }
                    MessageBox.Show("Баланс успешно отредактирован!",
                                    "Информация",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                    break;

                case Utils.EditBalanceErrors.UnexpectedError:
                    MessageBox.Show("Непредвиденная ошибка!",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                    break;
            }
        }

        private void RebalanceRadioButton_Click(object sender, RoutedEventArgs e)
        {
            BalanceTextBox.Text = Convert.ToString(user.Balance);
        }

        private void ReplenishmentRadioButton_Click(object sender, RoutedEventArgs e)
        {
            BalanceTextBox.Text = string.Empty;
        }


    }
}
