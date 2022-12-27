using System;
using RabbitMQ.Client;
using System.Text;

public class NewTask {
    public static void Main (string[] args) {
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

            string message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: "",
                routingKey: "worker-queue",
                basicProperties: null,
                body: body
            );

            Console.WriteLine($"[x] Sent {message}");
        }

        Console.WriteLine("Press [enter] to exit.");
        Console.ReadLine();
    }

    public static string GetMessage(string[] args) {
        return ((args.Length > 0)? string.Join(" ", args) : "Hello World!");
    }
}