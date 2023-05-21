using System.Data.SqlClient;

namespace EpicGames_KozlovSergey_ISP3120
{
    public static class Utils
    {
        public static string connectionString = @"Data Source=.\SQLEXPRESS; Initial Catalog=EpicGames_KozlovS_ISP3120; Integrated Security=True";
        public static string CurrentUserLogin = string.Empty;

        public static string dbErrorMessage = "Произошла ошибка с базой данных!";

        public static VM vm;

        public static bool CheckConnection()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException)
                {
                    return false;
                }
                return true;
            }
        }


        public enum LoginErrors
        {
            NoErrors,
            LoginIsEmpty,
            PasswordIsEmpty,
            UserDoesNotExist,
            IncorrectPassword,
            AccountIsDeactivated
        }

        public enum AddEditGameErrors
        {
            NoErrors,
            GameTitleIsEmpty,
            GameTitleIsLong,
            GameVersionIsLong,
            GamePriceHasLetters,
            GamePriceIsOutOfRange
        }

        public enum AddEditUserErrors
        {
            NoErrors,
            NoErrorsWithoutUpdatingPassword,
            LoginIsEmpty,
            PasswordIsEmpty,
            LoginMustHaveOtherCharacters,
            LoginIsTooShortOrLong,
            PasswordMustHaveDifferentCharacters,
            PasswordIsTooShortOrLong,
            SecondPasswordIsEmpty,
            PasswordDoesNotMatch,
            OldPasswordDoesNotMatch,
            UserExists,
            UserNicknameIsEmpty,
            UserNicknameIsShortOrLong
        }

        public enum EditBalanceMethod
        {
            Replenishment,
            Rebalance
        }

        public enum EditBalanceErrors
        {
            NoErrors,
            UnexpectedError,
            BalanceTextIsEmpty,
            BalanceTextHasLetters,
            BalanceNumberIsOutOfRange
        }
    }
}
