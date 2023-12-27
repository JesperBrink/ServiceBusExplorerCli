using FluentAssertions;
using Microsoft.Azure.ServiceBus.Management;
using Moq;
using ServiceBusExplorerCli.Repositories.Interface;
using ServiceBusExplorerCli.Services;
using Xunit;

namespace ServiceBusExplorerCli.Unittests.Services;

public class PubSubServiceTests
{
    private readonly PubSubService _uut;
    private readonly Mock<IServiceBusRepository> _serviceBusRepository;

    public PubSubServiceTests()
    {
        _serviceBusRepository = new Mock<IServiceBusRepository>();
        _uut = new PubSubService(_serviceBusRepository.Object);
    }
    
    // TODO:
    // Der er nu refactored så det burde være muligt at mocke og derved teste.
    // Næste step er derfor at lave testene i denne fil.
    // Derefter, lav lignende tests for QueueService.

    [Fact]
    public async Task Setup_InitiatesTopicAndSubscriptionNames()
    {
        // Arrange
        _serviceBusRepository
            .Setup(x => x.GetTopicsAsync())
            .ReturnsAsync(new List<TopicDescription> { new("Topic1"), new("Topic2") });

        _serviceBusRepository
            .Setup(x => x.GetSubscriptionsAsync("Topic1"))
            .ReturnsAsync(new List<SubscriptionDescription> { new("Topic1", "Sub1") });

        _serviceBusRepository
            .Setup(x => x.GetSubscriptionsAsync("Topic2"))
            .ReturnsAsync(new List<SubscriptionDescription>());

        var expectedDictionary = new Dictionary<string, List<string>>
        {
            {
                "Topic1",
                new List<string> { "Sub1" }
            }
        };

        // Act
        await _uut.Setup();

        // Assert
        var topicsAndSubscriptionNames = _uut.GetTopicsAndSubscriptionNames();
        topicsAndSubscriptionNames.Should().BeEquivalentTo(expectedDictionary);
    }

    [Fact]
    public void ResubmitDeadLetterMessages_ResubmitsDeadLettersForValidTopicAndSubscription()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact]
    public void ResubmitDeadLetterMessages_DoesNotChangeMessageIdByDefault()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact]
    public void ResubmitDeadLetterMessages_CreatesNewMessageIdWhenGivenFlag()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact]
    public void ResubmitDeadLetterMessages_ThrowsExceptionForUnknownTopicName()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact]
    public void ResubmitDeadLetterMessages_ThrowsExceptionForUnknownSubscriptionName()
    {
        // Arrange

        // Act

        // Assert
    }
}
