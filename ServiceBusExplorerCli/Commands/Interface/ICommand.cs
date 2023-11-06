namespace ServiceBusExplorerCli.Commands.Interface;

public interface ICommand
{
    string Title { get; }
    Task Execute();
}
