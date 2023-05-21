using EpicGames_KozlovSergey_ISP3120.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace EpicGames_KozlovSergey_ISP3120
{
    public static class Server
    {

        public static Users GetUser(string login)
        {
            Users newUser;
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM [Users] WHERE Login = \'{login}\'", sqlConnection);

                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    reader.Read();

                    // Конвертирование картинки из byte[] в ImageSource
                    ImageSource img = null;
                    if (reader["AvatarBin"] != DBNull.Value)
                    {
                        img = (ImageSource)new ImageSourceConverter().ConvertFrom(reader["AvatarBin"]);
                    }
                    string role = GetRole(login);

                    newUser = new Users()
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        Login = login,
                        Avatar = img,
                        Balance = Convert.ToDecimal(reader["Balance"]),
                        Nickname = Convert.ToString(reader["Nickname"]),
                        Role = GetRole(login),
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    };
                }
            }
            return newUser;
        }

        public static Utils.LoginErrors Auth(string login, string password)
        {
            if (login == string.Empty)
            {
                return Utils.LoginErrors.LoginIsEmpty;
            }
            else if (password == string.Empty)
            {
                return Utils.LoginErrors.PasswordIsEmpty;
            }

            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();

                SqlCommand command = new SqlCommand($"SELECT Password, IsActive FROM [Users] WHERE LOWER(Login) = \'{login}\'", sqlConnection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    // Если строка с введённым логином существует
                    if (!reader.HasRows)
                    {
                        return Utils.LoginErrors.UserDoesNotExist;
                    }

                    object objPassword = reader["Password"];
                    object objIsActive = reader["IsActive"];

                    // Шифрование пароля в MD5
                    MD5 md5 = new MD5CryptoServiceProvider();
                    byte[] checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                    string passResult = BitConverter.ToString(checkSum).Replace("-", string.Empty);

                    // Если пароль из базы данных соответствует введённому паролю
                    if (objPassword.ToString() == passResult)
                    {
                        // Если аккаунт деактивирован
                        if (Convert.ToBoolean(objIsActive) == false)
                        {
                            return Utils.LoginErrors.AccountIsDeactivated;
                        }
                        return Utils.LoginErrors.NoErrors;
                    }
                    else
                    {
                        return Utils.LoginErrors.IncorrectPassword;
                    }
                }
            }
        }

        public static Utils.AddEditUserErrors Registration(string login, string password, string secondPassword, bool isTest)
        {
            // Логин не зависит от регистра
            login = login.ToLower();

            // Если логин не введён
            if (login == string.Empty)
            {
                return Utils.AddEditUserErrors.LoginIsEmpty;
            }
            // Если в логине имеются символы, не перечисленные в регулярном выражении
            else if (!Regex.IsMatch(login, @"^[a-z0-9_.]*$"))
            {
                return Utils.AddEditUserErrors.LoginMustHaveOtherCharacters;
            }
            // Если длина логина не находится в промежутке от 3 до 20 включительно
            else if (login.Length > 20 || login.Length < 3)
            {
                return Utils.AddEditUserErrors.LoginIsTooShortOrLong;
            }
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand($"SELECT Login FROM [Users] WHERE LOWER(Login) = \'{login}\'", sqlConnection);

                // Если пользователь под данным логином уже существует
                if (sqlCommand.ExecuteScalar() != null)
                {
                    return Utils.AddEditUserErrors.UserExists;
                }
            }
            // Если пароль не введён
            if (password == string.Empty)
            {
                return Utils.AddEditUserErrors.PasswordIsEmpty;
            }
            // Если длина пароля не находится в промежутке от 8 до 32 включительно
            else if (password.Length < 8 || password.Length > 32)
            {
                return Utils.AddEditUserErrors.PasswordIsTooShortOrLong;
            }
            // Если в пароле отсутствуют строчные, прописные, не алфавитно-числовые символы или цифры
            else if (!(Regex.IsMatch(password, @"\W") &&
                       Regex.IsMatch(password, @"\d") &&
                       (Regex.IsMatch(password, @"[a-z]") || Regex.IsMatch(password, @"[а-я]")) &&
                       (Regex.IsMatch(password, @"[A-Z]") || Regex.IsMatch(password, @"[А-Я]"))))
            {
                return Utils.AddEditUserErrors.PasswordMustHaveDifferentCharacters;
            }
            // Если поле с повторным вводом пароля пустое
            else if (secondPassword == string.Empty)
            {
                return Utils.AddEditUserErrors.SecondPasswordIsEmpty;
            }
            // Если пароль не соответствует повторному вводу пароля
            else if (password != secondPassword)
            {
                return Utils.AddEditUserErrors.PasswordDoesNotMatch;
            }


            else
            {
                using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
                {
                    sqlConnection.Open();
                    // Если выполнение метода не является тестом
                    if (!isTest)
                    {
                        // Шифрование пароля в MD5
                        MD5 md5 = new MD5CryptoServiceProvider();
                        byte[] checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                        string PasswordResult = BitConverter.ToString(checkSum).Replace("-", string.Empty);

                        // Создание аккаунта
                        SqlCommand sqlCommand = new SqlCommand($"INSERT INTO [Users](Login, Password, Balance, Nickname, RoleID, IsActive) VALUES (\'{login}\', \'{PasswordResult}\', 0, \'{login}\', 1, 1)", sqlConnection);
                        sqlCommand.ExecuteNonQuery();
                    }

                }
                return Utils.AddEditUserErrors.NoErrors;
            }

        }

        public static string GetRole(string login)
        {
            string result;
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();
                result = new SqlCommand($"SELECT [Roles].Name FROM [Roles], [Users] " +
                                        $"WHERE LOWER(Login) = \'{login}\' " +
                                        $"AND [Roles].ID = [Users].RoleID",
                                        sqlConnection).ExecuteScalar().ToString();
            }
            return result;
        }

        public static Utils.AddEditGameErrors AddGame(string title, string creatorLogin, string description, string version, string price, byte[] image)
        {
            // Замена точки в цене на запятую, чтобы не было конфликтов с типом double
            price = price.Replace(".", ",");
            title = title.Trim().Replace("  ", " ");
            // Если название игры не введено
            if (string.IsNullOrWhiteSpace(title))
            {
                return Utils.AddEditGameErrors.GameTitleIsEmpty;
            }
            // Если название игры более 100 символов
            else if (title.Length > 100)
            {
                return Utils.AddEditGameErrors.GameTitleIsLong;
            }
            // Если название версии больше 50 символов
            else if (version.Length > 50)
            {
                return Utils.AddEditGameErrors.GameVersionIsLong;
            }
            // Если введённая цена не является числом и поле не пусто (если пустое, то игра бесплатная)
            else if (!double.TryParse(price, out double finalPrice) && !string.IsNullOrWhiteSpace(price))
            {
                return Utils.AddEditGameErrors.GamePriceHasLetters;
            }
            // Если цена не находится в промежутке от 0 до 10000 рублей
            else if (finalPrice < 0.00 || finalPrice > 10000.00)
            {
                return Utils.AddEditGameErrors.GamePriceIsOutOfRange;
            }
            else
            {
                // Время создания игры
                DateTime dateTime = DateTime.Now;
                string date = dateTime.ToString("yyyy-MM-dd");
                using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
                {
                    sqlConnection.Open();
                    // Получение id текущего пользователя с помощью имеющегося логина
                    SqlCommand sqlCommand = new SqlCommand($"SELECT ID FROM [Users] WHERE Login = \'{creatorLogin}\'", sqlConnection);
                    int creatorID = Convert.ToInt32(sqlCommand.ExecuteScalar());

                    //Создание игры
                    sqlCommand = new SqlCommand($"INSERT INTO [Games] VALUES (" +
                        $"\'{title}\'," +
                        $"\'{date}\'," +
                        $"{creatorID}," +
                        $"@imageData," +
                        $"@description," +
                        $"@version," +
                        $"@price," +
                        $"1)",
                        sqlConnection);


                    sqlCommand.Parameters.Add("@imageData", SqlDbType.Image);
                    sqlCommand.Parameters.Add("@description", SqlDbType.NVarChar);
                    sqlCommand.Parameters.Add("@version", SqlDbType.NVarChar);
                    sqlCommand.Parameters.Add("@price", SqlDbType.Decimal);
                    sqlCommand.Parameters["@imageData"].Value = (object)image ?? DBNull.Value;
                    sqlCommand.Parameters["@description"].Value = description;
                    sqlCommand.Parameters["@version"].Value = version;
                    sqlCommand.Parameters["@price"].Value = Math.Round(finalPrice, 2);

                    sqlCommand.ExecuteNonQuery();
                }
                return Utils.AddEditGameErrors.NoErrors;
            }
        }

        public static List<Games> GetGames()
        {
            List<Games> games = new List<Games>();

            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();
                // Получить список доступных игр
                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM [Games], [Users] WHERE [Games].CreatorID = [Users].ID AND [Games].IsAvailableInStore = 1 AND [Users].IsActive = 1", sqlConnection);

                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        ImageSource img = null;
                        // Конвертирование из byte[] в ImageSource
                        if (reader["LogoBin"] != DBNull.Value)
                        {
                            img = (ImageSource)new ImageSourceConverter().ConvertFrom(reader["LogoBin"]);
                        }

                        // Добавление игры в список
                        games.Add(new Games()
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Title = Convert.ToString(reader["Title"]),
                            Date = Convert.ToDateTime(reader["Date"]),
                            CreatorID = Convert.ToInt32(reader["CreatorID"]),
                            Logo = img,
                            Description = Convert.ToString(reader["Description"]),
                            Version = Convert.ToString(reader["Version"]),
                            price = Convert.ToDecimal(reader["Price"])
                        });
                    }
                }
            }
            // Возвращение списка доступных игр
            return games;
        }

        public static Games GetGame(int id)
        {
            Games game = new Games();
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM [Games] WHERE ID = {id}", sqlConnection);

                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    reader.Read();

                    // Конвертирование из byte[] в ImageSource
                    ImageSource img = null;
                    if (reader["LogoBin"] != DBNull.Value)
                    {
                        img = (ImageSource)new ImageSourceConverter().ConvertFrom(reader["LogoBin"]);
                    }

                    game = new Games()
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        Title = Convert.ToString(reader["Title"]),
                        Date = Convert.ToDateTime(reader["Date"]),
                        CreatorID = Convert.ToInt32(reader["CreatorID"]),
                        Logo = img,
                        Description = Convert.ToString(reader["Description"]),
                        Version = Convert.ToString(reader["Version"]),
                        price = Convert.ToDecimal(reader["Price"]),
                        IsAvailableInStore = Convert.ToBoolean(reader["IsAvailableInStore"])
                    };
                }
            }

            return game;
        }

        public static List<Users> GetUsers(string currentUserLogin)
        {
            List<Users> users = new List<Users>();

            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();
                // Получение списка пользователей кроме самого себя
                SqlCommand sqlCommand = new SqlCommand("SELECT [Users].ID, [Users].Login, [Users].AvatarBin, [Users].Balance, [Users].Nickname, [Roles].Name AS [RoleName], [Users].IsActive " +
                                                       $"FROM [Users], [Roles] WHERE [Users].RoleID = [Roles].ID AND Login != \'{currentUserLogin}\'", sqlConnection);

                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Конвертирование из byte[] в ImageSource
                        ImageSource img = null;
                        if (reader["AvatarBin"] != DBNull.Value)
                        {
                            img = (ImageSource)new ImageSourceConverter().ConvertFrom(reader["AvatarBin"]);
                        }



                        users.Add(new Users()
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Login = Convert.ToString(reader["Login"]),
                            Avatar = img,
                            Balance = Convert.ToDecimal(reader["Balance"]),
                            Nickname = Convert.ToString(reader["Nickname"]),
                            Role = Convert.ToString(reader["RoleName"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        });
                    }
                }
            }

            return users;
        }

        public static ImageSource GetUserImage(string login)
        {
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand($"SELECT AvatarBin FROM [Users] WHERE Login = \'{login}\'", sqlConnection);
                object imgObj = sqlCommand.ExecuteScalar();

                // Конвертирование из byte[] в ImageSource
                ImageSource img = null;
                if (imgObj != DBNull.Value)
                {
                    img = (ImageSource)new ImageSourceConverter().ConvertFrom(imgObj);
                }

                return img;
            }
        }

        public static decimal GetUserBalance(int id)
        {
            decimal userBalance;
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand($"SELECT Balance FROM [Users] WHERE ID = {id}", sqlConnection);
                userBalance = Convert.ToDecimal(sqlCommand.ExecuteScalar());
            }
            return userBalance;
        }

        public static string GetCreatorNickname(int id)
        {
            string creator;
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand($"SELECT Nickname FROM [Users] WHERE ID = {id}", sqlConnection);

                creator = sqlCommand.ExecuteScalar().ToString();
            }
            return creator;
        }

        public static string GetCreatorLogin(int id)
        {
            string creator;
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand($"SELECT Login FROM [Users] WHERE ID = {id}", sqlConnection);

                creator = sqlCommand.ExecuteScalar().ToString();
            }
            return creator;
        }

        public static Utils.AddEditGameErrors EditGame(string title, int gameID, string description, string version, string price, byte[] image)
        {
            // Замена точки в цене на запятую, чтобы не было конфликтов с типом double
            price = price.Replace(".", ",");
            title = title.Trim().Replace("  ", " ");
            version = version.Trim().Replace("  ", " ");
            // Если название игры не введено
            if (string.IsNullOrWhiteSpace(title))
            {
                return Utils.AddEditGameErrors.GameTitleIsEmpty;
            }
            // Если название игры более 100 символов
            else if (title.Length > 100)
            {
                return Utils.AddEditGameErrors.GameTitleIsLong;
            }
            // Если название версии больше 50 символов
            else if (version.Length > 50)
            {
                return Utils.AddEditGameErrors.GameVersionIsLong;
            }
            // Если введённая цена не является числом и поле не пусто(если пустое, то игра бесплатная)
            else if (!double.TryParse(price, out double finalPrice) && !string.IsNullOrWhiteSpace(price))
            {
                return Utils.AddEditGameErrors.GamePriceHasLetters;
            }
            // Если цена не находится в промежутке от 0 до 10000 рублей
            else if (finalPrice < 0.00 || finalPrice > 10000.00)
            {
                return Utils.AddEditGameErrors.GamePriceIsOutOfRange;
            }
            else
            {
                using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
                {
                    sqlConnection.Open();

                    SqlCommand sqlCommand = new SqlCommand($"UPDATE [Games] SET " +
                        $"Title = \'{title}\'," +
                        $"LogoBin = @imageData," +
                        $"Description = @description," +
                        $"Version = @version," +
                        $"Price = @price WHERE ID = {gameID}",
                        sqlConnection);


                    sqlCommand.Parameters.Add("@imageData", SqlDbType.Image);
                    sqlCommand.Parameters.Add("@description", SqlDbType.NVarChar);
                    sqlCommand.Parameters.Add("@version", SqlDbType.NVarChar);
                    sqlCommand.Parameters.Add("@price", SqlDbType.Decimal);
                    sqlCommand.Parameters["@imageData"].Value = (object)image ?? DBNull.Value;
                    sqlCommand.Parameters["@description"].Value = description;
                    sqlCommand.Parameters["@version"].Value = version;
                    sqlCommand.Parameters["@price"].Value = Math.Round(finalPrice, 2);

                    sqlCommand.ExecuteNonQuery();
                }
                return Utils.AddEditGameErrors.NoErrors;
            }
        }

        public static void HideGame(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();
                // Скрытие игры из магазина (она будет доступна только пользователям, купившим её ранее)
                SqlCommand sqlCommand = new SqlCommand($"UPDATE [Games] SET IsAvailableInStore = 0 WHERE ID = {id}", sqlConnection);

                sqlCommand.ExecuteNonQuery();
            }
        }

        public static byte[] GetGameImageByte(int id)
        {
            byte[] image = null;
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand($"SELECT LogoBin FROM [Games] WHERE ID = {id}", sqlConnection);

                image = (byte[])sqlCommand.ExecuteScalar();
            }
            return image;
        }

        public static void AddToWishlist(string currentUserLogin, int gameID)
        {
            // Время добавления в список желаемого
            DateTime dateTime = DateTime.Now;
            string date = dateTime.ToString();
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand($"INSERT INTO [Wishlists] VALUES ({GetUser(currentUserLogin).ID}, {gameID}, @date)", sqlConnection);

                sqlCommand.Parameters.Add("@date", SqlDbType.DateTime).Value = date;

                sqlCommand.ExecuteNonQuery();
            }
        }

        public static void RemoveFromWishlist(string currentUserLogin, int gameID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand($"DELETE FROM [Wishlists] WHERE UserID = {GetUser(currentUserLogin).ID} AND GameID = {gameID}", sqlConnection);

                sqlCommand.ExecuteNonQuery();
            }
        }

        public static bool IsGameInWishlist(string currentUserLogin, int gameID)
        {
            bool isInWishlist = false;
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand($"SELECT DateAdded FROM [Wishlists] WHERE UserID = {GetUser(currentUserLogin).ID} AND GameID = {gameID}", sqlConnection);

                if (sqlCommand.ExecuteScalar() != null)
                {
                    isInWishlist = true;
                }
            }
            return isInWishlist;
        }

        public static bool BuyGame(string currentUserLogin, int gameID)
        {
            bool isBuyGame = false;
            Users user = GetUser(currentUserLogin);
            Games game = GetGame(gameID);
            // Если баданс пользователя больше цены игры, то можно проводить операцию
            if (user.Balance >= game.price)
            {
                DateTime dateTime = DateTime.Now;
                string date = dateTime.ToString();
                using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
                {
                    sqlConnection.Open();

                    SqlCommand sqlCommand = new SqlCommand($"UPDATE [Users] SET Balance = @balance WHERE ID = {user.ID}", sqlConnection);
                    sqlCommand.Parameters.Add("@balance", SqlDbType.Decimal);
                    sqlCommand.Parameters["@balance"].Value = user.Balance - game.price;

                    //Если баланс был изменён
                    if (sqlCommand.ExecuteNonQuery() > 0)
                    {
                        sqlCommand = new SqlCommand($"INSERT INTO [Purchases] VALUES (" +
                            $"{user.ID}," +
                            $"{game.ID}," +
                            $"@price," +
                            $"@date)", sqlConnection);

                        sqlCommand.Parameters.Add("@price", SqlDbType.Decimal).Value = game.price;
                        sqlCommand.Parameters.Add("@date", SqlDbType.DateTime).Value = date;
                        sqlCommand.ExecuteNonQuery();
                        isBuyGame = true;
                    }
                }
            }
            return isBuyGame;
        }

        public static bool IsGameInLibrary(string currentUserLogin, int gameID)
        {
            bool isGameInLibrary = false;
            Users user = GetUser(currentUserLogin);
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand($"SELECT BuyDate FROM [Purchases] WHERE UserID = {user.ID} AND GameID = {gameID}", sqlConnection);

                if (sqlCommand.ExecuteScalar() != null)
                {
                    isGameInLibrary = true;
                }
            }
            return isGameInLibrary;
        }

        internal static void UpdateUserRole(string login, int role)
        {
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand($"UPDATE [Users] SET RoleID = {role} WHERE Login = \'{login}\'", sqlConnection);

                sqlCommand.ExecuteNonQuery();
            }
        }

        internal static List<Games> GetUserLibrary(int id)
        {
            List<Games> games = new List<Games>();

            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM [Games], [Purchases] WHERE [Purchases].GameID = [Games].ID AND [Purchases].UserID = {id}", sqlConnection);

                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Конвертирование из byte[] в ImageSource
                        ImageSource img = null;
                        if (reader["LogoBin"] != DBNull.Value)
                        {
                            img = (ImageSource)new ImageSourceConverter().ConvertFrom(reader["LogoBin"]);
                        }

                        games.Add(new Games()
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Title = Convert.ToString(reader["Title"]),
                            Date = Convert.ToDateTime(reader["Date"]),
                            CreatorID = Convert.ToInt32(reader["CreatorID"]),
                            Logo = img,
                            Description = Convert.ToString(reader["Description"]),
                            Version = Convert.ToString(reader["Version"]),
                            price = Convert.ToDecimal(reader["Price"]),
                            BuyDate = ((DateTime)reader["BuyDate"]).ToString("dd.MM.yyyy")
                        });
                    }
                }
            }

            return games;
        }

        internal static byte[] GetUserImageData(int id)
        {
            byte[] image = null;
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand($"SELECT AvatarBin FROM [Users] WHERE ID = {id}", sqlConnection);

                image = (byte[])sqlCommand.ExecuteScalar();
            }
            return image;
        }

        public static Utils.AddEditUserErrors EditUser(string nickname, int id, byte[] image, string oldPassword, string newPassword, string secondNewPassword)
        {
            // Убираются все пробелы из никнейма, кроме тех, которые между словами
            nickname = nickname.Trim().Replace("  ", " ");
            // Если ник пользователя не введён
            if (string.IsNullOrWhiteSpace(nickname))
            {
                return Utils.AddEditUserErrors.UserNicknameIsEmpty;
            }
            // Если ник пользователя не находится в промежутке от 3 до 32 символов
            else if (nickname.Length > 32 || nickname.Length < 3)
            {
                return Utils.AddEditUserErrors.UserNicknameIsShortOrLong;
            }
            // Если старый пароль введён
            else if (!string.IsNullOrEmpty(oldPassword))
            {
                // Шифрование пароля в MD5
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(oldPassword));
                string oldPasswordResult = BitConverter.ToString(checkSum).Replace("-", string.Empty);

                string oldPassFromDB;

                using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
                {
                    sqlConnection.Open();

                    SqlCommand sqlCommand = new SqlCommand($"SELECT Password FROM [Users] WHERE ID = {id}", sqlConnection);
                    oldPassFromDB = sqlCommand.ExecuteScalar().ToString();

                }

                // Если старый пароль из базы не соответствует введённому старому паролю
                if (oldPasswordResult != oldPassFromDB)
                {
                    return Utils.AddEditUserErrors.OldPasswordDoesNotMatch;
                }

                // Если новый пароль не введён
                if (string.IsNullOrEmpty(newPassword))
                {
                    return Utils.AddEditUserErrors.PasswordIsEmpty;
                }
                // Если новый пароль не находится в промежутке от 8 до 32 символов
                else if (newPassword.Length < 8 || newPassword.Length > 32)
                {
                    return Utils.AddEditUserErrors.PasswordIsTooShortOrLong;
                }
                // Если в пароле отсутствуют строчные, прописные, не алфавитно-числовые символы или цифры
                else if (!(Regex.IsMatch(newPassword, @"\W") &&
                        Regex.IsMatch(newPassword, @"\d") &&
                        (Regex.IsMatch(newPassword, @"[a-z]") || Regex.IsMatch(newPassword, @"[а-я]")) &&
                        (Regex.IsMatch(newPassword, @"[A-Z]") || Regex.IsMatch(newPassword, @"[А-Я]"))))
                {
                    return Utils.AddEditUserErrors.PasswordMustHaveDifferentCharacters;
                }
                // Если повторный новый пароль не введён
                else if (string.IsNullOrEmpty(secondNewPassword))
                {
                    return Utils.AddEditUserErrors.SecondPasswordIsEmpty;
                }
                // Если новые пароли не совпадают
                else if (newPassword != secondNewPassword)
                {
                    return Utils.AddEditUserErrors.PasswordDoesNotMatch;
                }
                else
                {


                    // Шифрование пароля в MD5
                    checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
                    string PasswordResult = BitConverter.ToString(checkSum).Replace("-", string.Empty);

                    using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
                    {
                        sqlConnection.Open();



                        SqlCommand sqlCommand = new SqlCommand($"UPDATE [Users] SET " +
                                                                $"Nickname = \'{nickname}\'," +
                                                                $"AvatarBin = @imageData," +
                                                                $"Password = \'{PasswordResult}\' " +
                                                                $"WHERE ID = {id}", sqlConnection);

                        sqlCommand.Parameters.Add("@imageData", SqlDbType.Image);
                        sqlCommand.Parameters["@imageData"].Value = (object)image ?? DBNull.Value;

                        sqlCommand.ExecuteNonQuery();
                    }
                    return Utils.AddEditUserErrors.NoErrors;
                }
            }
            // Иначе можно обновлять данные пользователя без изменения пароля
            else
            {
                using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
                {
                    sqlConnection.Open();



                    SqlCommand sqlCommand = new SqlCommand($"UPDATE [Users] SET " +
                                                            $"Nickname = \'{nickname}\'," +
                                                            $"AvatarBin = @imageData " +
                                                            $"WHERE ID = {id}", sqlConnection);

                    sqlCommand.Parameters.Add("@imageData", SqlDbType.Image);
                    sqlCommand.Parameters["@imageData"].Value = (object)image ?? DBNull.Value;

                    sqlCommand.ExecuteNonQuery();
                }
                return Utils.AddEditUserErrors.NoErrorsWithoutUpdatingPassword;
            }
        }

        internal static void DeactivateUser(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand($"UPDATE [Users] SET IsActive = 0 WHERE ID = {id}", sqlConnection);

                sqlCommand.ExecuteNonQuery();
            }
        }

        internal static void ActivateUser(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand($"UPDATE [Users] SET IsActive = 1 WHERE ID = {id}", sqlConnection);

                sqlCommand.ExecuteNonQuery();
            }
        }

        internal static Utils.EditBalanceErrors EditUserBalance(int id, string text, Utils.EditBalanceMethod method)
        {
            // Максимально допустимый баланс на аккаунте
            decimal maxBalance = 100000m;
            // Замена точки в цене на запятую, чтобы не было конфликтов с типом decimal
            text = text.Replace(".", ",");
            // Если поле баланса пустое
            if (string.IsNullOrWhiteSpace(text))
            {
                return Utils.EditBalanceErrors.BalanceTextIsEmpty;
            }
            // Если в поле содержатся не числовые значения
            else if (!decimal.TryParse(text, out decimal finalSum))
            {
                return Utils.EditBalanceErrors.BalanceTextHasLetters;
            }
            // Если число не находится в промежутке от 0 до максимально допустимого значения
            else if (finalSum < 0m || finalSum > maxBalance)
            {
                return Utils.EditBalanceErrors.BalanceNumberIsOutOfRange;
            }
            else
            {
                // Если метод изменение баланса - пополнение (текущий баланс + введённая сумма)
                if (method == Utils.EditBalanceMethod.Replenishment)
                {
                    finalSum += GetUserBalance(id);
                    // Если финальная сумма больше текущего баланса
                    if (finalSum > maxBalance)
                    {
                        return Utils.EditBalanceErrors.BalanceNumberIsOutOfRange;
                    }
                }

                // Boolean изменился ли баланс
                bool changes = false;
                using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
                {
                    sqlConnection.Open();

                    SqlCommand sqlCommand = new SqlCommand($"UPDATE [Users] SET Balance = @balance WHERE ID = {id}", sqlConnection);
                    sqlCommand.Parameters.Add("@balance", SqlDbType.Decimal);
                    sqlCommand.Parameters["@balance"].Value = Math.Round(finalSum, 2);

                    // При возврате числа не равного нулю возвращается значение true
                    changes = Convert.ToBoolean(sqlCommand.ExecuteNonQuery());
                }
                // Если изменения произошли
                if (changes)
                {
                    return Utils.EditBalanceErrors.NoErrors;
                }
                else
                {
                    return Utils.EditBalanceErrors.UnexpectedError;
                }
            }
        }

        internal static int[] GetUserWishlist(int userID)
        {
            int[] gameIDs;
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand($"SELECT COUNT(*) FROM [Wishlists] WHERE UserID = {userID}", sqlConnection);

                //Создание массива с количеством игр, заранее указанном в массиве
                gameIDs = new int[Convert.ToInt32(sqlCommand.ExecuteScalar())];

                sqlCommand.CommandText = $"SELECT GameID FROM [Wishlists] WHERE UserID = {userID}";

                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    int lines = 0;
                    while (reader.Read())
                    {
                        // Получение id игры и добавление в массив
                        gameIDs[lines] = reader.GetInt32(0);
                        lines++;
                    }
                }
            }
            return gameIDs;
        }

        internal static bool DeleteGameFromLibrary(int userID, int gameID)
        {
            using (SqlConnection sqlConnection = new SqlConnection(Utils.connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand($"DELETE FROM [Purchases] WHERE GameID = {gameID} AND UserID = {userID}", sqlConnection);

                return Convert.ToBoolean(sqlCommand.ExecuteNonQuery());
            }
        }
    }
}