using EpicGames_KozlovSergey_ISP3120.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;

namespace EpicGames_KozlovSergey_ISP3120.Entities
{
    public class Games
    {
        ImageSource logo;
        public bool emptyLogo = true;
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int CreatorID { get; set; }
        public string BuyDate { get; set; }
        public bool IsAvailableInStore { get; set; }
        public ImageSource Logo
        {
            get
            {
                return logo;
            }
            set
            {
                if (value == null)
                {
                    Image image = Resources.GamePlaceholder;
                    using (var ms = new MemoryStream())
                    {
                        image.Save(ms, image.RawFormat);
                        logo = (ImageSource)new ImageSourceConverter().ConvertFrom(ms.ToArray());
                    }
                    emptyLogo = true;
                }
                else
                {
                    logo = value;
                    emptyLogo = false;
                }
            }
        }

        public string description;
        public string Description
        {
            get
            {
                if (description == string.Empty)
                {
                    return "Пусто";
                }
                else
                {
                    return description;
                }
            }
            set
            {
                description = value;
            }
        }
        public string Version { get; set; }
        public decimal price;

        public string Price
        {
            get
            {
                if (price == 0)
                {
                    return "Бесплатно";
                }
                else
                {
                    return price + " руб.";
                }
            }
        }

        public string Creator
        {
            get
            {
                return $"{Server.GetCreatorNickname(CreatorID)} ({Server.GetCreatorLogin(CreatorID)})";
            }
        }

        public string GetDate
        {
            get
            {
                return Date.ToString("dd.MM.yyyy");
            }
        }


        public string EditVisibilityString
        {
            get
            {
                if (CreatorID == Utils.vm.CurrentUser.ID || Utils.vm.CurrentUser.Role == "Модератор" || Utils.vm.CurrentUser.Role == "Администратор")
                {
                    return "Visible";
                }
                else
                {
                    return "Collapsed";
                }
            }
        }

        public string BuyButtonVisibilityString
        {
            get
            {
                if (Server.IsGameInLibrary(Utils.CurrentUserLogin, ID) || !IsAvailableInStore)
                {
                    return "Collapsed";
                }
                else
                {
                    return "Visible";
                }
            }
        }

        public string IsVersionExistsVisibilityString
        {
            get
            {
                return Version == "" ? "Hidden" : "Visible";
            }
        }
    }
}
