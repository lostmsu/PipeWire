using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using PipeWire;

var pw = await PipeWireLib.Load();
if (!pw.IsAvailable) {
    Console.Error.WriteLine("PipeWire dynamic library was not found");
    return -8;
}

Console.WriteLine("PipeWire loaded");
pw.Init();
Console.WriteLine("PipeWire initialized");

var mainLoop = pw.CreateMainLoop();
Console.WriteLine("created main loop");

var loop = pw.GetLoop(mainLoop);
Console.WriteLine("got loop");

var pwProps = pw.CreateProperties(PropertyKey.MEDIA_TYPE, MediaType.VIDEO);
Console.WriteLine("created properties");
pw.Set(pwProps, PropertyKey.MEDIA_CATEGORY, MediaCategory.CAPTURE);
pw.Set(pwProps, PropertyKey.MEDIA_ROLE, MediaRole.SCREEN);

using var stream = pw.CreateStream("capture", loop, pwProps, new EventHandler());
Console.WriteLine("created stream");

if (pw.ConnectStream(stream)) {
    Console.WriteLine("Connected to screen capture node");

    // Start receiving frames
    stream.Activate();

    // Keep running until user presses Enter
    int result = pw.Run(mainLoop);

    // Clean up
    stream.Deactivate();
} else {
    Console.Error.WriteLine("Failed to connect to screen capture node");
    return -7;
}

return 0;


void ProcessFrame(GCHandle userData) {
    if (pw.DequeueBuffer(stream) is not { } buffer)
        return;

    // Print frame information
    Console.WriteLine($"Received frame: Buffer={buffer.buffer:X}, " +
                      $"Data count={buffer.n_datas}, " +
                      $"Data pointer={buffer.datas:X}");

    // Return the buffer to PipeWire
    pw.QueueBuffer(stream, buffer);
}

class EventHandler: IStreamEventVisitor {
    public void OnDestroy() => LogName();

    public void OnDrained() => LogName();

    public void OnParamChanged(SpaParam param, SpaPodPtr pod) {
        Console.WriteLine("Changed: " + param);
    }

    int processCalls = 0;

    public void OnProcess() {
        int calls = Interlocked.Increment(ref this.processCalls);
        Console.WriteLine($"Processing frame {calls}");
    }

    public void OnStateChanged(StreamState old, StreamState state, string? error) {
        Console.WriteLine($"State changed: {old} -> {state} ({error})");
    }

    static void LogName([CallerMemberName] string name = "") => Console.WriteLine(name);
}