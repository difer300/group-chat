using System.Web;

namespace GroupChat.Models
{
    public sealed class Session
    {
        public User CurrentUser
        {
            get =>
                HttpContext.Current.Session["User"] == null ? null : (User)HttpContext.Current.Session["User"];

            set => HttpContext.Current.Session["User"] = value;
        }

        public User BotUser
        {
            get =>
                HttpContext.Current.Session["BotUser"] == null ? null : (User)HttpContext.Current.Session["BotUser"];

            set => HttpContext.Current.Session["BotUser"] = value;
        }
        public Group CurrentGroup
        {
            get =>
                HttpContext.Current.Session["Group"] == null ? null : (Group)HttpContext.Current.Session["Group"];

            set => HttpContext.Current.Session["Group"] = value;
        }
    }
}