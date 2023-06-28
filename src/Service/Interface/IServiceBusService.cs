public interface IServiceBusService
{
    public Task Setup();
    public IReadOnlyList<string> GetQueueNames();
}
