using GroupChat.DAL;
using GroupChat.Models;
using GroupChat.Settings;
using PusherServer;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace GroupChat.Controllers
{
    public class AuthController : Controller
    {
        private readonly GroupChatContext _db = new GroupChatContext();
        private readonly Pusher _pusher;
        private readonly Session _session = new Session();

        public AuthController()
        {
            var options = new PusherOptions
            {
                Cluster = AppSettings.Cluster,
                Encrypted = true
            };

            _pusher = new Pusher(
                AppSettings.AppId,
                AppSettings.AppKey,
                AppSettings.AppSecret,
                options);
        }

        public ActionResult Index()
        {
            if (_session.CurrentUser != null)
            {
                return Redirect($"/Chat");
            }

            return View();
        }

        public JsonResult AuthForChannel(string channel_name, string socket_id)
        {
            if (_session.CurrentUser == null)
            {
                return Json(new { status = HttpStatusCode.BadRequest, message = ErrorMessages.UserNotLogged });
            }

            int groupId;
            try
            {
                groupId = int.Parse(channel_name.Replace(AppSettings.PartialGroupName, ""));
            }
            catch (FormatException e)
            {
                return Json(new { Content = e.Message });
            }

            var isInChannel = _db.UserGroups.Count(gb => gb.GroupId == groupId
                                                         && gb.UserId == _session.CurrentUser.ID);

            if (isInChannel <= 0) return Json(new { Content = ErrorMessages.ForbiddenError });
            var auth = _pusher.Authenticate(channel_name, socket_id);

            return Json(auth);
        }

        [HttpPost]
        public ActionResult Login()
        {
            var userName = Request.Form["username"];
            if (userName.Trim() == "")
            {
                return Redirect("/");
            }

            var user = _db.Users.FirstOrDefault(u => u.Name == userName);
            if (user == null)
            {
                user = CreateNewUser(userName);
            }

            _session.CurrentUser = user;

            return Redirect($"/Chat");
        }

        [HttpPost]
        public ActionResult Logout()
        {
            _session.CurrentUser = null;
            return Json(new { redirectToUrl = Url.Action("Index", "Auth") });
        }

        private User CreateNewUser(string userName)
        {
            var user = new User { Name = userName };
            _db.Users.Add(user);
            _db.SaveChanges();
            AddUserToGroups(user.ID);
            return user;
        }

        private void AddUserToGroups(int userId)
        {
            var groups = _db.Groups.ToList();
            foreach (var g in groups)
            {
                _db.UserGroups.Add(new UserGroup() { GroupId = g.ID, UserId = userId });
                _db.SaveChanges();
            }
        }
    }
}