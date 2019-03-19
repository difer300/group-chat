using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using GroupChat.Controllers;
using GroupChat.DAL;
using GroupChat.Models;
using GroupChat.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using PusherServer;
using System.Collections.Specialized;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using Newtonsoft.Json;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace GroupChat.Tests
{
    [TestClass]
    public class ChatControllerTests
    {
        private ChatController _controller;
        private Session _session;
        private IFixture _fixture;
        private Mock<IGroupChatContext> _context;
        private Mock<IPusher> _pusher;

        [SetUp]
        public void SetUp()
        {
            _pusher = new Mock<IPusher>();
            _session = new Session();
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            LoadHttContext();
            LoadValidData();
            LoadUrlAction();
        }

        [Test]
        public void Index_WithCurrentUserSession_RedirectToChat()
        {
            //arrange
            _session.CurrentUser = _fixture.Create<User>();

            //act
            var response = _controller.Index();

            //assert
            Assert.AreEqual("", ((ViewResult)response).ViewName);
        }

        [Test]
        public void Index_WithoutCurrentUserSession_RedirectToHome()
        {
            //arrange
            _session.CurrentUser = null;

            //act
            var response = _controller.Index();

            //assert
            Assert.AreEqual("/", ((RedirectResult)response).Url);
        }

        [Test]
        public void SendMessage_WithoutCurrentUserSession_ResponseUserNotLoggedError()
        {
            //arrange
            _session.CurrentUser = null;

            //act
            var response = _controller.SendMessage();

            //assert
            var errorMessage = (ErrorMessage)response.Data;
            Assert.AreEqual(HttpStatusCode.BadRequest, errorMessage.Status);
            Assert.AreEqual(ErrorMessages.UserNotLogged, errorMessage.Message);
        }

        [Test]
        public void SendMessage_WithValidMessageRequest_ResponseValidMessage()
        {
            //arrange
            _session.CurrentUser = _fixture.Create<User>();
            _session.CurrentGroup = _fixture.Create<Group>();
            LoadFormRequest();

            //act
            var response = _controller.SendMessage();

            //assert
            var message = (Message)response.Data;
            Assert.IsNotNull(message);
            Assert.AreEqual("TestMessage", message.Text);
        }

        public static HttpContext FakeHttpContext()
        {
            var httpRequest = new HttpRequest("", "http://localhost/", "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(), new HttpStaticObjectsCollection(), 10, true, HttpCookieMode.AutoDetect, SessionStateMode.InProc, false);

            SessionStateUtility.AddHttpSessionStateToContext(httpContext, sessionContainer);

            return httpContext;
        }

        private static DbSet<T> GetQueryableMockDbSet<T>(params T[] sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return dbSet.Object;
        }

        private void LoadHttContext()
        {
            HttpContext.Current = FakeHttpContext();
        }

        private void LoadValidData()
        {
            var mockUser = GetQueryableMockDbSet(
                new User() { ID = 1, Name = "Test" }
            );
            var mockGroups = GetQueryableMockDbSet(
                new Group() { ID = 1, Name = "Work" }
            );
            var mockUsersGroup = GetQueryableMockDbSet(
                new UserGroup() { ID = 1, GroupId = 1, UserId = 1 }
            );
            var mockMessages = GetQueryableMockDbSet(
                new Message() { AddedBy = mockUser.First(), CreatedDate = DateTime.Now, Group = mockGroups.First(), Text = "Hi guys!!", IsBoot = false }
            );

            _context = new Mock<IGroupChatContext>();
            _context.Setup(x => x.Users).Returns(mockUser);
            _context.Setup(x => x.Groups).Returns(mockGroups);
            _context.Setup(x => x.UserGroups).Returns(mockUsersGroup);
            _context.Setup(x => x.Messages).Returns(mockMessages);

            _controller = new ChatController(_context.Object, _pusher.Object);

            NameValueCollection form = new NameValueCollection();
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(frm => frm.HttpContext.Request.Form).Returns(form);
            _controller.ControllerContext = controllerContext.Object;
        }

        private void LoadUrlAction()
        {
            Mock<UrlHelper> urlHelperMock = new Mock<UrlHelper>();
            _controller.Url = urlHelperMock.Object;
            urlHelperMock.Setup(x => x.Action("Index", "Auth")).Returns("testUrl");
        }

        private void LoadFormRequest()
        {
            NameValueCollection form = new NameValueCollection();
            form["username"] = "Test";
            form["message"] = "TestMessage";
            form["socket_id"] = "1";
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(frm => frm.HttpContext.Request.Form).Returns(form);
            _controller.ControllerContext = controllerContext.Object;
        }
    }
}
