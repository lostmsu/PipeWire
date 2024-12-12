namespace PipeWire;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public sealed class StreamEventHandler: IDisposable {
    internal unsafe StreamEvents* Events { get; private set; }
    GCHandle handle;
    readonly IStreamEventVisitor visitor;

    internal unsafe StreamEventHandler(IStreamEventVisitor visitor) {
        this.visitor = visitor ?? throw new ArgumentNullException(nameof(visitor));

        this.Events = (StreamEvents*)Marshal.AllocHGlobal(sizeof(StreamEvents));
        Marshal.StructureToPtr(new StreamEvents {
            Version = StreamEventsVersion.STREAM_EVENTS,

            Destroy = &Destroy,
            Drained = &OnDrained,
            ParamChanged = &OnParamChanged,
            Process = &OnProcess,
            StateChanged = &OnStateChanged,
        }, (IntPtr)this.Events, fDeleteOld: false);

        this.handle = GCHandle.Alloc(visitor);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void Destroy(GCHandle handle) {
        var visitor = (IStreamEventVisitor)handle.Target!;
        visitor.OnDestroy();
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void OnDrained(GCHandle handle) {
        var visitor = (IStreamEventVisitor)handle.Target!;
        visitor.OnDrained();
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void OnParamChanged(GCHandle handle, SpaParam id, SpaPodPtr param) {
        var visitor = (IStreamEventVisitor)handle.Target!;
        visitor.OnParamChanged(id, param);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void OnProcess(GCHandle handle) {
        var visitor = (IStreamEventVisitor)handle.Target!;
        visitor.OnProcess();
    }
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static unsafe void OnStateChanged(GCHandle handle, StreamState old, StreamState state, byte* error) {
        var visitor = (IStreamEventVisitor)handle.Target!;
        visitor.OnStateChanged(old, state, Marshal.PtrToStringAnsi((IntPtr)error));
    }

    unsafe void ReleaseUnmanagedResources() {
        if (this.Events == null)
            return;

        this.handle.Free();
        Marshal.FreeHGlobal((IntPtr)this.Events);
        this.Events = null;
    }

    public void Dispose() {
        this.ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~StreamEventHandler() {
        this.ReleaseUnmanagedResources();
    }
}