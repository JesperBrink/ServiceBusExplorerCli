# ServiceBusExplorerCli

## TODOs - Queue:
- [x] Retrieve list of all queues
- [x] Retrieve list of messages in queue
- [x] Retrieve list of deadletter messages in queue
- [x] Resubmit deadletter messages

## TODOs - Topics:
- [x] Retrieve list of all topics and their subscriptions
- [x] Retrieve list of messages in topic/subscription 
- [x] Retrieve list of deadletter messages in topic/subscription
- [x] Resubmit deadletter messages 

## TODOs - General
- [ ] CLI
- [ ] "How to" guide
- [ ] Setup test project
- [ ] Make sure the CLI informs the user about the fact that resubmitting deadletters will affect ALL subscriptions in the topic.
- [ ] Make functionality to get all queues and/or topics with deadletter messages
- [ ] When resubmitting deadletters, it should be possible to generate a new id for the message.
- [ ] Investigate possibility of avoiding to resubmit to all subscriptions in a topic, then resubmitting deadletter messages.
