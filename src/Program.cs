// using Service.ServiceBusService;

var connectionString = "<connectionString>"
var serviceBusService = new ServiceBusService(connectionString);

Console.WriteLine("ServiceBusService initiated");

await serviceBusService.Setup();
var queueNames = serviceBusService.GetQueueNames();

foreach (var queueName in queueNames)
{
    Console.WriteLine($"Queue: {queueName}\n");

    var messages = await serviceBusService.PeekMessagesInQueue(queueName, 10);
    Console.WriteLine($"Number of messages in queue: {messages.Count}");
    foreach (var message in messages)
    {
        Console.WriteLine(message.Body);
    }

    Console.WriteLine();

    var dlqMessages = await serviceBusService.PeekDeadLetterMessagesInQueue(queueName, 10);
    Console.WriteLine($"Number of messages in DeadletterQueue: {dlqMessages.Count}");
    foreach (var message in dlqMessages)
    {
        Console.WriteLine(message.Body);
    }

    Console.WriteLine("\n##########################");
}
