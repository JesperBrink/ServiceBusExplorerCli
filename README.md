# ServiceBusExplorerCli

This project is a small and simple tool for interacting with [Azure Service Bus](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview). The purpose is not to have all the same features as [Service Bus Explorer](https://github.com/paolosalvatori/ServiceBusExplorer), but instead to be a simple cross-platform alternative with a few essential features for interacting with Azure Service Bus. 

## Supported commands
- [x] List the names of all queues or topics.
- [x] List the names of all the subscriptions in a topic.
- [x] List all queues/subscriptions with messages in their DeadLetterQueue.
- [x] Peek messages.
- [x] Peek deadletter messages.
- [x] Resubmit deadletter messages.
      
## Future work
- Improved CLI experience.
- More error handling.
- Allow adding more than 1 ConnectionConfig
- When resubmitting deadletters, it should be possible to generate a new id for the message.
- Investigate possibility for only resubmitting to a single subscription instead of resubmitting to all subscriptions in a topic, when resubmitting deadletter messages.
- Refactoring some of the command logic out to avoid duplicated logic in multiple commands.
