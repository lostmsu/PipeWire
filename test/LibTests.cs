namespace PipeWire;

public class LibTests {
    [Fact]
    public async Task IsAvailable() {
        var pw = await PipeWireLib.Load();
        Assert.True(pw.IsAvailable);
    }

    [Fact]
    public async Task Init() {
        var pw = await PipeWireLib.Load();
        pw.Init();
    }
}