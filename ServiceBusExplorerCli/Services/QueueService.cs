using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using ServiceBusExplorerCli.Exceptions;
using ServiceBusExplorerCli.Repositories.Interface;
using ServiceBusExplorerCli.Services.Interface;

namespace ServiceBusExplorerCli.Services;

public class QueueService : IQueueService
{
    private readonly IServiceBusRepository _serviceBusRepository;
    private IReadOnlyList<string> _queueNames = new List<string>();
    private IDictionary<string, ServiceBusReceiver> _receiverLookUp =
        new Dictionary<string, ServiceBusReceiver>();
    private IDictionary<string, ServiceBusSender> _senderLookUp =
        new Dictionary<string, ServiceBusSender>();

    public QueueService(IServiceBusRepository serviceBusRepository)
    {
        _serviceBusRepository = serviceBusRepository;
    }

    public async Task Setup()
    {
        _queueNames = await RetrieveQueueNames();
        _receiverLookUp = CreateReceivers(_queueNames);
        _senderLookUp = CreateSenders(_queueNames);
    }

    public IReadOnlyList<string> GetQueueNames() => _queueNames;

    public async Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekMessages(
        string queueName,
        int noOfMessages
    )
    {
        var receiver = GetReceiverOrThrow(queueName);
        return await receiver.PeekMessagesAsync(noOfMessages);
    }

    public async Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekDeadLetterMessages(
        string queueName,
        int noOfMessages
    )
    {
        var receiver = GetDeadLetterReceiverOrThrow(queueName);
        return await receiver.PeekMessagesAsync(noOfMessages);
    }

    public async Task ResubmitDeadLetterMessages(
        string queueName,
        int fetchCount,
        TimeSpan? maxWaitTime = null,
        bool createNewMessageId = false
    )
    {
        maxWaitTime ??= new TimeSpan(0, 0, 10);

        var receiver = GetDeadLetterReceiverOrThrow(queueName);
        var sender = GetSenderOrThrow(queueName);

        var deadLetterMessages = await receiver.ReceiveMessagesAsync(fetchCount, maxWaitTime);

        foreach (var message in deadLetterMessages)
        {
            var resubmittableMessage = new ServiceBusMessage(message);

            if (createNewMessageId)
            {
                var newMessageId = Guid.NewGuid();
                resubmittableMessage.MessageId = newMessageId.ToString();
            }

            await sender.SendMessageAsync(resubmittableMessage);
            await receiver.CompleteMessageAsync(message);
        }
    }

    private ServiceBusSender GetSenderOrThrow(string queueName)
    {
        if (!_senderLookUp.TryGetValue(queueName, out var sender))
        {
            throw new NotFoundException(
                $"No ServiceBusSender was found for queue named '{queueName}'."
            );
        }

        return sender;
    }

    private ServiceBusReceiver GetReceiverOrThrow(string queueName)
    {
        if (!_receiverLookUp.TryGetValue(queueName, out var receiver))
        {
            throw new NotFoundException(
                $"No ServiceBusReceiver was found for queue named '{queueName}'."
            );
        }

        return receiver;
    }

    private ServiceBusReceiver GetDeadLetterReceiverOrThrow(string queueName)
    {
        var deadLetterQueueName = GetDeadLetterQueuePath(queueName);
        if (!_receiverLookUp.TryGetValue(deadLetterQueueName, out var receiver))
        {
            throw new NotFoundException(
                $"No ServiceBusReceiver was found for queue named '{deadLetterQueueName}'."
            );
        }

        return receiver;
    }

    private async Task<IReadOnlyList<string>> RetrieveQueueNames()
    {
        var queues = await _serviceBusRepository.GetQueuesAsync();
        return queues.Select(q => q.Path).ToList();
    }

    private IDictionary<string, ServiceBusReceiver> CreateReceivers(IReadOnlyList<string> queues)
    {
        var lookUp = new Dictionary<string, ServiceBusReceiver>();

        foreach (var queue in queues)
        {
            var receiver = _serviceBusRepository.CreateReceiver(queue);
            lookUp.Add(queue, receiver);

            var deadLetterQueue = GetDeadLetterQueuePath(queue);
            var deadLetterReceiver = _serviceBusRepository.CreateReceiver(deadLetterQueue);
            lookUp.Add(deadLetterQueue, deadLetterReceiver);
        }

        return lookUp;
    }

    private IDictionary<string, ServiceBusSender> CreateSenders(IReadOnlyList<string> queues)
    {
        var lookUp = new Dictionary<string, ServiceBusSender>();

        foreach (var queue in queues)
        {
            var sender = _serviceBusRepository.CreateSender(queue);
            lookUp.Add(queue, sender);
        }

        return lookUp;
    }

    private static string GetDeadLetterQueuePath(string queueName)
    {
        return $"{queueName}/$deadletterqueue";
    }
}
