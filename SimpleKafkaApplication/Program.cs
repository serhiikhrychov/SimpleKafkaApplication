using System.Collections;
using System.Diagnostics;
using Confluent.Kafka;
using System.Linq;

namespace SimpleKafkaApplication
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var kafkaServer = "localhost:9092";
            var kafkaTopic = "my_topic";
            var messageValue = $"Hello {DateTime.Now.ToString()}";
            
            ProduceMessage(messageValue, kafkaTopic, kafkaServer);
            
            var messageValueFromConsumer = ConsumerMessage(kafkaTopic, kafkaServer);

            Debug.Assert(messageValue == messageValueFromConsumer);
        }

        private static void ProduceMessage(string messageValue, string kafkaTopic, string kafkaServer)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = kafkaServer
            };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                var message = new Message<Null, string> {Value = messageValue};
                // Produce a message to Kafka
                var result = producer.ProduceAsync(kafkaTopic, message).GetAwaiter().GetResult();

                Console.WriteLine($"Message {messageValue} produced to topic {result.Topic}");
            }
        }

        private static string ConsumerMessage(string kafkaTopic, string kafkaServer)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = kafkaServer,
                GroupId = "my_group_id",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SessionTimeoutMs = 6000,
            };

            // Create a Kafka consumer
            IEnumerable<ConsumeResult<Ignore, string>> Enumerable()
            {
                using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
                {
                    consumer.Subscribe(kafkaTopic);

                    while (true)
                        yield return consumer.Consume(TimeSpan.FromSeconds(1));
                }
            }

            // Consume messages from Kafka
            var consumeResult = Enumerable().SkipWhile(x => x == null).TakeWhile(x => x != null).Last();
            Console.WriteLine($"Message {consumeResult.Message.Value} consumed from topic {consumeResult.Topic}");
            return consumeResult.Message.Value;
        }
    }
}
