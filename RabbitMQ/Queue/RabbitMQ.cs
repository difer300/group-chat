using System;
using System.Text;
using GroupChat.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Queue
{
    public class RabbitMQ
    {
        public IConnection GetConnection()
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = MQSettings.UserName,
                Password = MQSettings.Password,
                Port = MQSettings.Port,
                HostName = MQSettings.HostName,
                VirtualHost = MQSettings.VirtualHost
            };

            return factory.CreateConnection();
        }
        public bool Send(IConnection con, string message, string queue)
        {
            try
            {
                var channel = con.CreateModel();
                channel.ExchangeDeclare("messageexchange", ExchangeType.Direct);
                channel.QueueDeclare(queue, true, false, false, null);
                channel.QueueBind(queue, "messageexchange", queue, null);
                var msg = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("messageexchange", queue, null, msg);

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public string Receive(IConnection con, string queue)
        {
            try
            {
                var channel = con.CreateModel();
                channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                var consumer = new EventingBasicConsumer(channel);
                BasicGetResult result = channel.BasicGet(queue: queue, autoAck: true);
                return result != null ? Encoding.UTF8.GetString(result.Body) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}