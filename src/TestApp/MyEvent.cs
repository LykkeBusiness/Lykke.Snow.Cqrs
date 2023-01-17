using MessagePack;

namespace TestApp;

[MessagePackObject()]
public class MyEvent
{
    [Key(0)]
    public string Test { get; set; }
}