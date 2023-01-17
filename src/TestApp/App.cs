using Lykke.Cqrs;

namespace TestApp;

public class App
{
    private readonly ICqrsEngine _cqrsEngine;
    private readonly CqrsContextNamesSettings _names;

    public App(ICqrsEngine cqrsEngine, CqrsContextNamesSettings names)
    {
        _cqrsEngine = cqrsEngine;
        _names = names;
    }
    
    public void Execute()
    {
        _cqrsEngine.PublishEvent(new MyEvent() { Test = "123"}, _names.MyService);
        _cqrsEngine.SendCommand(new MyEvent() {Test = "456"}, _names.MyService, _names.MyService);
    }
}