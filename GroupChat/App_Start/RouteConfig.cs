using System.Web.Mvc;
using System.Web.Routing;

namespace GroupChat
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Home",
                url: "",
                defaults: new { controller = "Auth", action = "Index" }
            );

            routes.MapRoute(
                name: "PusherAuth",
                url: "pusher/auth",
                defaults: new { controller = "Auth", action = "AuthForChannel" }
            );

            routes.MapRoute(
                name: "Login",
                url: "login",
                defaults: new { controller = "Auth", action = "Login" }
            );

            routes.MapRoute(
                name: "Logout",
                url: "logout",
                defaults: new { controller = "Auth", action = "Logout" }
            );

            routes.MapRoute(
                name: "Chat",
                url: "chat",
                defaults: new { controller = "Chat", action = "Index" }
            );

            routes.MapRoute(
                name: "LoadChatWithGroupId",
                url: "group/messages/{id}",
                defaults: new { controller = "Chat", action = "LoadChatWithGroupId", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SendMessage",
                url: "messages/send",
                defaults: new { controller = "Chat", action = "SendMessage" }
            );

            routes.MapRoute(
                name: "SendCommandMessageMq",
                url: "mq/send",
                defaults: new { controller = "Chat", action = "SendCommandMessageMq" }
            );

            routes.MapRoute(
                name: "ReceiveCommandMessageMq",
                url: "mq/receive",
                defaults: new { controller = "Chat", action = "ReceiveCommandMessageMq" }
            );
        }
    }
}
