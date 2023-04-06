using Confluent.Kafka;

namespace SimpleKafkaApplication
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var kafkaServer = "localhost:9092";
            var kafkaTopic = "my_topic";

            var config = new ProducerConfig
            {
                BootstrapServers = kafkaServer
            };

            // Create a Kafka producer
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                var messageValue = $"Hello {DateTime.Now.ToString()}";

                var message = new Message<Null, string> {Value = messageValue};
                // Produce a message to Kafka
                var result = producer.ProduceAsync(kafkaTopic, message).GetAwaiter().GetResult();
                
                Console.WriteLine($"Message produced to topic {result.Topic} at partition {result.Partition} with offset {result.Offset}");
                


                var consumerConfig = new ConsumerConfig
                {
                    BootstrapServers = kafkaServer,
                    GroupId = "my_group_id",
                    AutoOffsetReset = AutoOffsetReset.Latest
                };

                // Create a Kafka consumer
                using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
                {
                    consumer.Subscribe(kafkaTopic);

                    // Consume messages from Kafka
                    var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));

                    // Print the message value to the console
                    Console.WriteLine(consumeResult != null
                        ? consumeResult.Message.Value
                        : "No new messages on the topic.");
                }
            }
        }
    }
}