using System.Threading;
using System;
using System.Net;
using System.Linq;
using Confluent.Kafka;
using System.Threading.Tasks;

namespace kafka
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cancel = new CancellationTokenSource();
            var topic = "quickstart-events";

            if (args.Length == 0)
            {
                args = new[] { "autoproducer", "consumer" };
            }

            Task consumer = null;
            if (args.Any(p => p.ToLower() == "consumer"))
            {
                consumer = Task.Run(() =>
                {
                    var config = new ConsumerConfig
                    {
                        BootstrapServers = "localhost:9092",
                        GroupId = System.Guid.NewGuid().ToString(),
                        AutoOffsetReset = AutoOffsetReset.Earliest,
                        EnableAutoCommit = false,
                    };

                    using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
                    {
                        consumer.Subscribe(topic);

                        while (!cancel.Token.IsCancellationRequested)
                        {
                            try
                            {
                                var consumeResult = consumer.Consume(cancel.Token);
                                var msg = consumeResult.Message;
                                Console.WriteLine($"{msg.Timestamp.UtcDateTime}: {msg.Value}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }

                        consumer.Close();
                    }
                });
            }

            if (args.Any(p => p.ToLower() == "autoproducer"))
            {
                var producerConfig = new ProducerConfig
                {
                    BootstrapServers = "192.168.1.40:9092",
                    ClientId = Dns.GetHostName(),
                };
                await Task.Run(() =>
                        {
                            using (var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
                            {
                                for (var i = 0; i < 10; i++)
                                {
                                    var value = $"ValueA {i}";
                                    producer.Produce(topic, new Message<Null, string>
                                    {
                                        Value = value,
                                    });
                                    Console.WriteLine(value);
                                    producer.Flush(TimeSpan.FromSeconds(10));
                                }
                            }
                        });
            }

            if (args.Any(p => p.ToLower() == "producer"))
            {
                var producerConfig = new ProducerConfig
                {
                    BootstrapServers = "192.168.1.40:9092",
                    ClientId = Dns.GetHostName(),
                };
                await Task.Run(async () =>
                            {
                                using (var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
                                {
                                    while (!cancel.Token.IsCancellationRequested)
                                    {
                                        Console.Write(">");
                                        var value = Console.ReadLine();
                                        await producer.ProduceAsync(topic, new Message<Null, string>
                                        {
                                            Value = value,
                                        });
                                    }
                                    producer.Flush(TimeSpan.FromSeconds(10));
                                }
                            });
            }

            if (args.Length == 1)
            {
                consumer?.Wait();
            }
            else
            {
                consumer?.Wait(30000);
            }
        }
    }
}
