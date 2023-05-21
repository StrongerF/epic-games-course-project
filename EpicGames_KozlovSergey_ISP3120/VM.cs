using System.Windows;

namespace EpicGames_KozlovSergey_ISP3120
{
    public class VM
    {
        public Entities.Users CurrentUser { get; set; }

        public Visibility CreatorControlsVisibility
        {
            get
            {
                if (CreatorControlsVisibilityString == "Visible")
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }



        public string AdminControlsVisibilityString
        {
            get
            {
                if (CurrentUser.Role == "Администратор")
                {
                    return "Visible";
                }
                return "Collapsed";
            }
        }
        public string ModeratorControlsVisibilityString
        {
            get
            {
                switch (CurrentUser.Role)
                {
                    case "Администратор":
                    case "Модератор":
                        return "Visible";
                    default:
                        return "Collapsed";
                }
            }
        }

        public string CreatorControlsVisibilityString
        {
            get
            {
                switch (CurrentUser.Role)
                {
                    case "Администратор":
                    case "Модератор":
                    case "Разработчик игр":
                        return "Visible";
                    default:
                        return "Collapsed";
                }
            }
        }
    }
}
