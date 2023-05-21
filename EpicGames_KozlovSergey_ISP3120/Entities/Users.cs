using EpicGames_KozlovSergey_ISP3120.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;

namespace EpicGames_KozlovSergey_ISP3120.Entities
{
    public class Users
    {
        public int ID { get; set; }
        public string Login { get; set; }
        public ImageSource avatar;
        public bool IsActive { get; set; }
        public string IsNotActiveVisibilityString
        {
            get
            {
                if (IsActive)
                {
                    return "Collapsed";
                }
                return "Visible";
            }
        }
        public string IsActiveVisibilityString
        {
            get
            {
                if (IsActive)
                {
                    return "Visible";
                }
                return "Collapsed";
            }
        }


        public bool emptyAvatar = true;
        public ImageSource Avatar
        {
            get
            {
                return avatar;
            }
            set
            {
                if (value == null)
                {
                    Image image = Resources.UserPlaceholder;
                    using (var ms = new MemoryStream())
                    {
                        image.Save(ms, image.RawFormat);
                        avatar = (ImageSource)new ImageSourceConverter().ConvertFrom(ms.ToArray());
                    }
                    emptyAvatar = true;
                }
                else
                {
                    avatar = value;
                    emptyAvatar = false;
                }
            }
        }

        public string BalanceString
        {
            get
            {
                return Balance + " руб.";
            }
        }
        public decimal Balance { get; set; }
        public string Nickname { get; set; }
        public string Role { get; set; }

        public string SelectedRoleIndexString
        {
            get
            {
                if (Role == "Администратор")
                {
                    return "3";
                }
                else if (Role == "Модератор")
                {
                    return "2";
                }
                else if (Role == "Разработчик игр")
                {
                    return "1";
                }
                else if (Role == "Пользователь")
                {
                    return "0";
                }
                else
                {
                    return "-1";
                }
            }
            set
            {
                if (value == "3")
                {
                    Role = "Администратор";
                }
                else if (value == "2")
                {
                    Role = "Модератор";
                }
                else if (value == "1")
                {
                    Role = "Разработчик игр";
                }
                else if (value == "0")
                {
                    Role = "Пользователь";
                }
                else
                {
                    Role = "";
                }
                Server.UpdateUserRole(Login, Convert.ToInt32(value) + 1);


            }
        }

        public string HideCurrentAccount
        {
            get
            {
                if (Login == Utils.CurrentUserLogin)
                {
                    return "Collapsed";
                }
                else
                {
                    return "Visible";
                }
            }
        }

        public string ModeratorVisibility
        {
            get
            {
                if (Utils.vm.CurrentUser.Role == "Администратор")
                {
                    return "Visible";
                }
                else
                {
                    return "Collapsed";
                }
            }
        }

        public string HigherRoleVisibility
        {
            get
            {
                if (Convert.ToInt32(Utils.vm.CurrentUser.SelectedRoleIndexString) <= Convert.ToInt32(SelectedRoleIndexString))
                {
                    return "Collapsed";
                }
                else
                {
                    return "Visible";
                }
            }
        }
    }
}
