using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rabbitmq
{
    internal class RabbitMethods
    {
        public static void SendMessage(string message)
        {
            // Configuração da conexão
            var factory = new ConnectionFactory() { HostName = "localhost" }; //IP ou hostname do RabbitMQ(vai na porta padrão por default)
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declarar uma fila
            channel.QueueDeclare(queue: "apicomm",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            // Mensagem a ser enviada
            var body = Encoding.UTF8.GetBytes(message);

            // Enviar mensagem para a fila
            channel.BasicPublish(exchange: "",
                                 routingKey: "apicomm",
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine($"Sent message: {message}");
        }

        public static void ReceiveMessage()
        {
            // Configuração da conexão
            var factory = new ConnectionFactory() { HostName = "localhost" }; // IP ou hostname do RabbitMQ (vai na porta padrão por default)
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declarar a fila
            channel.QueueDeclare(queue: "apicomm",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            Console.WriteLine("Waiting for messages");

            // Consumir mensagens
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received: {message}");
            };

            channel.BasicConsume(queue: "apicomm",
                                 autoAck: true,
                                 consumer: consumer);

            // Mantém aberto
            Console.ReadLine();
        }
    }
}
