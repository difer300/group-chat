using System.Net;

namespace GroupChat.Models
{
    public class ErrorMessage
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
    }
}