// using Service.ServiceBusService;

var queueService = new QueueService(connectionString);
Console.WriteLine("QueueService initiated");

await queueService.Setup();
var queueNames = queueService.GetQueueNames();

foreach (var queueName in queueNames)
{
    Console.WriteLine($"Queue: {queueName}\n");

    var messages = await queueService.PeekMessages(queueName, 10);
    Console.WriteLine($"Number of messages in queue: {messages.Count}");
    foreach (var message in messages)
    {
        Console.WriteLine(message.Body);
    }

    Console.WriteLine();

    var dlqMessages = await queueService.PeekDeadLetterMessages(queueName, 10);
    Console.WriteLine($"Number of messages in DeadletterQueue: {dlqMessages.Count}");
    foreach (var message in dlqMessages)
    {
        Console.WriteLine(message.Body);
    }

    Console.WriteLine("\n##########################");
}
