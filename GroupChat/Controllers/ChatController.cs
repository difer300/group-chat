using CsvHelper;
using CsvHelper.Configuration;
using GroupChat.DAL;
using GroupChat.Models;
using GroupChat.Settings;
using Newtonsoft.Json;
using PusherServer;
using RabbitMQ.Client;
using System;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GroupChat.Controllers
{
    public class ChatController : Controller
    {
        private readonly IGroupChatContext _db;
        private readonly Session _session;
        private IPusher _pusher;

        public ChatController()
        {
            _db = new GroupChatContext();
            _session = new Session();
            InitPusher();
        }

        public ChatController(IGroupChatContext context, IPusher pusher)
        {
            _db = context;
            _session = new Session();
            _pusher = pusher;
        }

        public ActionResult Index()
        {
            if (_session.CurrentUser == null)
            {
                return Redirect("/");
            }

            ViewBag.allGroups = _db.Groups.ToList();
            ViewBag.currentUser = _session.CurrentUser;

            return View();
        }

        public JsonResult LoadChatWithGroupId(int id)
        {
            if (_session.CurrentUser == null)
            {
                return Json(new { status = HttpStatusCode.BadRequest, message = ErrorMessages.UserNotLogged });
            }

            _session.CurrentGroup = _db.Groups.FirstOrDefault(g => g.ID == id);
            _session.BotUser = _db.Users.FirstOrDefault(u => u.Name == AppSettings.BotUser);

            var messages = _db.Messages
                    .Where(m => m.Group.ID == id)
                    .Include(m => m.AddedBy).ToList()
                    .Select(x => new Message
                    {
                        ID = x.ID,
                        AddedBy = x.IsBoot ? _session.BotUser : x.AddedBy,
                        CreatedDate = x.CreatedDate,
                        Group = x.Group,
                        Text = x.Text
                    })
                    .OrderByDescending(m => m.CreatedDate)
                    .Take(AppSettings.LimitLoadMessages)
                    .OrderBy(m => m.CreatedDate)
                    .ToList();

            return Json(new { status = HttpStatusCode.OK, data = messages }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SendMessage()
        {
            if (_session.CurrentUser == null)
            {
                return Json(new ErrorMessage
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = ErrorMessages.UserNotLogged
                });
            }

            Message message;
            if (Request.Form["message"].Equals(AppSettings.CommandChat))
            {
                message = new Message()
                {
                    AddedBy = null,
                    AddedBy_ID = _session.BotUser.ID,
                    Group = null,
                    Group_ID = _session.CurrentGroup.ID,
                    CreatedDate = DateTime.Now,
                    IsBoot = true
                };

                var json = JsonConvert.SerializeObject(message);
                SendCommandMessageMq(json, _session.CurrentUser.ID.ToString());
            }
            else
            {
                message = new Message()
                {
                    AddedBy = null,
                    AddedBy_ID = _session.CurrentUser.ID,
                    Text = Request.Form["message"],
                    Group = null,
                    Group_ID = _session.CurrentGroup.ID,
                    CreatedDate = DateTime.Now,
                    IsBoot = false
                };

                _db.Messages.Add(message);
                _db.SaveChanges();
            }

            message.AddedBy = _session.CurrentUser;
            message.Group = _session.CurrentGroup;

            _pusher.TriggerAsync(
                AppSettings.PartialGroupName + message.Group.ID,
                AppSettings.NewMessageEvent,
                message,
                new TriggerOptions() { SocketId = Request.Form["socket_id"] }
            );

            return Json(message);

        }

        [HttpPost]
        public JsonResult SendCommandMessageMq(string message, string user)
        {
            var obj = new Queue.RabbitMQ();
            IConnection con = obj.GetConnection();
            var flag = obj.Send(con, message, user);
            return Json(null);
        }

        [HttpPost]
        public JsonResult ReceiveCommandMessageMq()
        {
            try
            {
                var obj = new Queue.RabbitMQ();
                var con = obj.GetConnection();
                var userQueue = _session.CurrentUser.ID.ToString();
                var data = obj.Receive(con, userQueue);
                var message = JsonConvert.DeserializeObject<Message>(data);
                message.Text = GetBotMessage();
                return Json(message);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<string> GetExternalApiResponse()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(AppSettings.StooqUrl);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }

        private string GetBotMessage()
        {
            var result = GetExternalApiResponse();
            decimal price = 0;
            using (var tsr = new StringReader(result.Result))
            using (var csvReader = new CsvReader(tsr, new Configuration { HasHeaderRecord = true }))
            {
                while (csvReader.Read())
                {
                    string[] row = csvReader.Context.Record;
                    price = Convert.ToDecimal(row[1], CultureInfo.InvariantCulture);
                }
            }

            return string.Format(AppSettings.BotMessage, price);
        }

        private void InitPusher()
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
    }
}
