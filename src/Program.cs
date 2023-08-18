// using Service.ServiceBusService;

var connectionString = "<connectionString>"
var serviceBusService = new ServiceBusService(connectionString);

Console.WriteLine("ServiceBusService initiated");

await serviceBusService.Setup();
var queueNames = serviceBusService.GetQueueNames();
foreach (var queueName in queueNames)
{
    Console.WriteLine(queueName);
}
