using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

public class Worker {
    public static void Main() {
        var factory = new ConnectionFactory() {
            HostName = "localhost",
            Port = 5672
        };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel()) {
            channel.QueueDeclare( 
                queue: "worker-queue",
                durable: false, 
                exclusive: false,
                autoDelete: false, 
                arguments: null
            );

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) => {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received {message}");

                // Fake Task:
                int dots = message.Split('.').Length - 1;
                Thread.Sleep(dots * 1000);

                Console.WriteLine($" [x] Task Done");
            };

            channel.BasicConsume(
                queue: "worker-queue",  // routingKey
                autoAck: true,
                consumer: consumer
            );

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}