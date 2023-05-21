using EpicGames_KozlovSergey_ISP3120;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EpicGamesUnitTest
{
    [TestClass]
    public class RegistrationUnitTest
    {
        [TestMethod]
        public void Login__Password__SecondPassword__Result_LoginIsEmpty()
        {
            string login = "";
            string password = "";
            string secondPassword = "";

            Utils.AddEditUserErrors result = Server.Registration(login, password, secondPassword, true);
            Utils.AddEditUserErrors expected = Utils.AddEditUserErrors.LoginIsEmpty;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Login_s_Password__SecondPassword__Result_LoginIsTooShortOrLong()
        {
            string login = "s";
            string password = "";
            string secondPassword = "";

            Utils.AddEditUserErrors result = Server.Registration(login, password, secondPassword, true);
            Utils.AddEditUserErrors expected = Utils.AddEditUserErrors.LoginIsTooShortOrLong;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Login_абвгд_Password__SecondPassword__Result_LoginMustHaveOtherCharacters()
        {
            string login = "абвгд";
            string password = "";
            string secondPassword = "";

            Utils.AddEditUserErrors result = Server.Registration(login, password, secondPassword, true);
            Utils.AddEditUserErrors expected = Utils.AddEditUserErrors.LoginMustHaveOtherCharacters;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Login__Password_123_SecondPassword__Result_LoginIsEmpty()
        {
            string login = "";
            string password = "123";
            string secondPassword = "";

            Utils.AddEditUserErrors result = Server.Registration(login, password, secondPassword, true);
            Utils.AddEditUserErrors expected = Utils.AddEditUserErrors.LoginIsEmpty;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Login_1234567890ABCDEFGHIJKLMNOPQRST_Password__SecondPassword__Result_LoginIsTooShortOrLong()
        {
            // Логин должен быть до 20 символов, здесь 30 символов
            string login = "1234567890ABCDEFGHIJKLMNOPQRST";
            string password = "";
            string secondPassword = "";

            Utils.AddEditUserErrors result = Server.Registration(login, password, secondPassword, true);
            Utils.AddEditUserErrors expected = Utils.AddEditUserErrors.LoginIsTooShortOrLong;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Login_12345678_Password__SecondPassword__Result_PasswordIsEmpty()
        {
            string login = "12345678";
            string password = "";
            string secondPassword = "";

            Utils.AddEditUserErrors result = Server.Registration(login, password, secondPassword, true);
            Utils.AddEditUserErrors expected = Utils.AddEditUserErrors.PasswordIsEmpty;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Login_12345678_Password_1_SecondPassword__Result_PasswordIsTooShortOrLong()
        {
            string login = "12345678";
            // Пароль содержит 7 символов (<8)
            string password = "1234567";
            string secondPassword = "";

            Utils.AddEditUserErrors result = Server.Registration(login, password, secondPassword, true);
            Utils.AddEditUserErrors expected = Utils.AddEditUserErrors.PasswordIsTooShortOrLong;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Login_12345678_Password_12345678901234567890123456789012_SecondPassword__Result_PasswordIsTooShortOrLong()
        {
            string login = "12345678";
            // Пароль содержит 33 символа (>32)
            string password = "123456789012345678901234567890123";
            string secondPassword = "";

            Utils.AddEditUserErrors result = Server.Registration(login, password, secondPassword, true);
            Utils.AddEditUserErrors expected = Utils.AddEditUserErrors.PasswordIsTooShortOrLong;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Login_12345678_Password_123абвгд_SecondPassword__Result_PasswordMustHaveDifferentCharacters()
        {
            string login = "12345678";
            // Отсутствуют не алфавитно-цифровые символы и прописные буквы
            string password = "123абвгд";
            string secondPassword = "";

            Utils.AddEditUserErrors result = Server.Registration(login, password, secondPassword, true);
            Utils.AddEditUserErrors expected = Utils.AddEditUserErrors.PasswordMustHaveDifferentCharacters;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Login_12345678_Password_123абвгдAndExclamationPoint_SecondPassword__Result_PasswordMustHaveDifferentCharacters()
        {
            string login = "12345678";
            // Отсутствуют прописные буквы
            string password = "123абвгд!";
            string secondPassword = "";

            Utils.AddEditUserErrors result = Server.Registration(login, password, secondPassword, true);
            Utils.AddEditUserErrors expected = Utils.AddEditUserErrors.PasswordMustHaveDifferentCharacters;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Login_12345678_Password_123Абвгд_SecondPassword__Result_PasswordMustHaveDifferentCharacters()
        {
            string login = "12345678";
            // Отсутствуют не алфавитно-цифровые символы
            string password = "123Абвгд";
            string secondPassword = "";

            Utils.AddEditUserErrors result = Server.Registration(login, password, secondPassword, true);
            Utils.AddEditUserErrors expected = Utils.AddEditUserErrors.PasswordMustHaveDifferentCharacters;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Login_12345678_Password_123АбвгдAndExclamationPoint_SecondPassword__Result_SecondPasswordIsEmpty()
        {
            string login = "12345678";
            string password = "123Абвгд!";
            string secondPassword = "";

            Utils.AddEditUserErrors result = Server.Registration(login, password, secondPassword, true);
            Utils.AddEditUserErrors expected = Utils.AddEditUserErrors.SecondPasswordIsEmpty;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Login_12345678_Password_123АбвгдAndExclamationPoint_SecondPassword_1_Result_PasswordDoesNotMatch()
        {
            string login = "12345678";
            string password = "123Абвгд!";
            string secondPassword = "1";

            Utils.AddEditUserErrors result = Server.Registration(login, password, secondPassword, true);
            Utils.AddEditUserErrors expected = Utils.AddEditUserErrors.PasswordDoesNotMatch;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Login_sergey59420_Password_123АбвгдAndExclamationPoint_SecondPassword_123АбвгдAndExclamationPoint_Result_UserExists()
        {
            // Логин существующего пользователя
            string login = "sergey59420";
            string password = "123Абвгд!";
            string secondPassword = "123Абвгд!";

            Utils.AddEditUserErrors result = Server.Registration(login, password, secondPassword, true);
            Utils.AddEditUserErrors expected = Utils.AddEditUserErrors.UserExists;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Login_12345678_Password_123АбвгдAndExclamationPoint_SecondPassword_123АбвгдAndExclamationPoint_Result_NoErrors()
        {
            string login = "12345678";
            string password = "123Абвгд!";
            string secondPassword = "123Абвгд!";

            Utils.AddEditUserErrors result = Server.Registration(login, password, secondPassword, true);
            Utils.AddEditUserErrors expected = Utils.AddEditUserErrors.NoErrors;
            Assert.AreEqual(expected, result);
        }
    }
}
