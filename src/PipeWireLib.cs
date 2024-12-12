namespace PipeWire;

using System.IO;
using System.Runtime.InteropServices;

using Nix;

[StructLayout(LayoutKind.Sequential)]
struct SpaRectangle {
    public uint width;
    public uint height;
}

[StructLayout(LayoutKind.Sequential)]
struct SpaFraction {
    public uint num;
    public uint denom;
}

[StructLayout(LayoutKind.Sequential)]
struct SpaVideoInfo {
    public uint flags;
    public SpaRectangle size;
    public SpaFraction framerate;
}

[StructLayout(LayoutKind.Sequential)]
struct PwBufferHeader {
    public uint flags;
    public uint offset;
    public uint size;
    public ulong timestamp;
}

[StructLayout(LayoutKind.Sequential)]
public struct PwBuffer {
    public IntPtr buffer;
    public IntPtr datas;
    public uint n_datas;
}

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void PwStreamCallback(GCHandle userData);

public sealed class PipeWireLib {
    const string LIB_PIPE_WIRE = "libpipewire-0.3.so.0";
    const string NIX_PACKAGE_PREFIX = "pipewire-1.";

    readonly IntPtr handle;

    public bool IsAvailable => this.handle != IntPtr.Zero;

    unsafe PipeWireLib(IntPtr handle) {
        this.handle = handle;
        if (handle == IntPtr.Zero)
            return;

        this.pw_init = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void>)
            NativeLibrary.GetExport(handle, nameof(this.pw_init));

        this.pw_main_loop_new = (delegate* unmanaged[Cdecl]<IntPtr, MainLoopPtr>)
            NativeLibrary.GetExport(handle, nameof(this.pw_main_loop_new));

        this.pw_main_loop_get_loop = (delegate* unmanaged[Cdecl]<MainLoopPtr, LoopPtr>)
            NativeLibrary.GetExport(handle, nameof(this.pw_main_loop_get_loop));

        this.pw_main_loop_run = (delegate* unmanaged[Cdecl]<MainLoopPtr, int>)
            NativeLibrary.GetExport(handle, nameof(this.pw_main_loop_run));

        this.pw_stream_new_simple =
            (delegate* unmanaged[Cdecl]
                <LoopPtr, StrPtr, PropertiesPtr, StreamEvents*, GCHandle, StreamPtr>)
            NativeLibrary.GetExport(handle, nameof(this.pw_stream_new_simple));

        this.pw_stream_connect =
            (delegate* unmanaged[Cdecl]<StreamPtr, StreamDirection, uint, StreamFlags, SpaPodPtr*,
                uint, int>)
            NativeLibrary.GetExport(handle, nameof(this.pw_stream_connect));

        this.pw_stream_destroy = (delegate* unmanaged[Cdecl]<StreamPtr, void>)
            NativeLibrary.GetExport(handle, nameof(this.pw_stream_destroy));

        this.pw_stream_dequeue_buffer = (delegate* unmanaged[Cdecl]<StreamPtr, PwBuffer*>)
            NativeLibrary.GetExport(handle, nameof(this.pw_stream_dequeue_buffer));

        this.pw_stream_queue_buffer = (delegate* unmanaged[Cdecl]<StreamPtr, PwBuffer*, int>)
            NativeLibrary.GetExport(handle, nameof(this.pw_stream_queue_buffer));

        this.pw_stream_set_active = (delegate* unmanaged[Cdecl]<StreamPtr, bool, int>)
            NativeLibrary.GetExport(handle, nameof(this.pw_stream_set_active));

        this.pw_stream_add_listener =
            (delegate* unmanaged[Cdecl]<StreamPtr, SpaHookPtr, StreamEvents*, GCHandle, void>)
            NativeLibrary.GetExport(handle, nameof(this.pw_stream_add_listener));

        this.pw_properties_new =
            (delegate* unmanaged[Cdecl]<StrPtr, StrPtr, NullPtr, PropertiesPtr>)
            NativeLibrary.GetExport(handle, nameof(this.pw_properties_new));

        this.pw_properties_set =
            (delegate* unmanaged[Cdecl]<PropertiesPtr, StrPtr, StrPtr, int>)
            NativeLibrary.GetExport(handle, nameof(this.pw_properties_set));
    }

    public unsafe void Init() {
        this.EnsureAvailable();
        this.pw_init(IntPtr.Zero, IntPtr.Zero);
    }

    public unsafe MainLoopPtr CreateMainLoop() {
        this.EnsureAvailable();
        return this.pw_main_loop_new(IntPtr.Zero);
    }

    public unsafe LoopPtr GetLoop(MainLoopPtr mainLoop) {
        this.EnsureAvailable();
        return this.pw_main_loop_get_loop(mainLoop);
    }

    public unsafe int Run(MainLoopPtr mainLoop) {
        this.EnsureAvailable();
        return this.pw_main_loop_run(mainLoop);
    }

    public unsafe PropertiesPtr CreateProperties(string key, string value) {
        this.EnsureAvailable();
        using var keyPtr = new StrPtr(key);
        using var valuePtr = new StrPtr(value);
        return this.pw_properties_new(keyPtr, valuePtr, default);
    }

    public unsafe bool Set(PropertiesPtr properties, string key, string value) {
        this.EnsureAvailable();
        using var keyPtr = new StrPtr(key);
        using var valuePtr = new StrPtr(value);
        int result = this.pw_properties_set(properties, keyPtr, valuePtr);
        return result switch {
            1 => true,
            0 => false,
            _ => throw new InvalidOperationException(),
        };
    }

    public unsafe PipeWireStream CreateStream(string name, LoopPtr loop, PropertiesPtr properties,
                                              IStreamEventVisitor events) {
        this.EnsureAvailable();
        var dataHandle = GCHandle.Alloc(events, GCHandleType.Pinned);
        using var namePtr = new StrPtr(name);
        var handler = new StreamEventHandler(events);
        var stream = this.pw_stream_new_simple(
            loop,
            namePtr,
            properties, // Default properties
            handler.Events, // default events
            dataHandle
        );

        if (stream == default) {
            handler.Dispose();
            throw new InvalidOperationException("Failed to create PipeWire stream");
        }

        return new(this, stream);
    }

    public unsafe bool ConnectStream(PipeWireStream stream, uint targetID) {
        this.EnsureAvailable();

        int result = this.pw_stream_connect(
            stream.Handle,
            StreamDirection.INPUT,
            targetID,
            StreamFlags.AUTO_CONNECT,
            null,
            0
        );

        return result == 0;
    }
    
    public bool ConnectStream(PipeWireStream stream) {
        return this.ConnectStream(stream, 0xffffffff);
    }

    public unsafe void SetStreamActive(PipeWireStream stream, bool active) {
        this.EnsureAvailable();
        int result = this.pw_stream_set_active(stream.Handle, active);
        if (result != 0)
            throw new InvalidOperationException($"Failed to set stream active state to {active}");
    }

    public unsafe PwBuffer? DequeueBuffer(PipeWireStream stream) {
        this.EnsureAvailable();
        var bufferPtr = this.pw_stream_dequeue_buffer(stream.Handle);
        if (bufferPtr == default)
            return null;

        return *bufferPtr;
    }

    public unsafe bool QueueBuffer(PipeWireStream stream, PwBuffer buffer) {
        this.EnsureAvailable();
        return this.pw_stream_queue_buffer(stream.Handle, &buffer) == 0;
    }

    public unsafe void DestroyStream(StreamPtr stream) {
        if (stream == default)
            throw new ArgumentNullException(nameof(stream));
        this.EnsureAvailable();
        this.pw_stream_destroy(stream);
    }

    public static Task<PipeWireLib> Load() {
        lock (LoadLock) {
            return load ??= LoadImpl();
        }
    }

    void EnsureAvailable() {
        if (!this.IsAvailable)
            throw new InvalidOperationException("PipeWire library is not available");
    }

    static Task<PipeWireLib>? load;
    static readonly object LoadLock = new();

    static async Task<PipeWireLib> LoadImpl() {
        try {
            return new(NativeLibrary.Load(LIB_PIPE_WIRE));
        } catch (DllNotFoundException) {
            if (await NixStore.GetCurrentSystem().ConfigureAwait(false) is not { } nix)
                throw;

            await foreach (var package in nix.GetAllDependencies()) {
                if (package.Name.StartsWith(NIX_PACKAGE_PREFIX)) {
                    string path = Path.Join(package.Path.FullName, "lib", LIB_PIPE_WIRE);
                    return new(NativeLibrary.Load(path));
                }
            }

            throw;
        }
    }

    // ReSharper disable InconsistentNaming
    unsafe delegate* unmanaged[Cdecl]<IntPtr, IntPtr, void> pw_init;

    unsafe delegate* unmanaged[Cdecl]
        <LoopPtr, StrPtr, PropertiesPtr, StreamEvents*, GCHandle, StreamPtr>
        pw_stream_new_simple;

    // struct pw_main_loop * pw_main_loop_new 	( 	const struct spa_dict * 	props	)
    unsafe delegate* unmanaged[Cdecl]<IntPtr, MainLoopPtr> pw_main_loop_new;

    unsafe delegate* unmanaged[Cdecl]<StreamPtr, StreamDirection, uint, StreamFlags, SpaPodPtr*,
        uint, int>
        pw_stream_connect;

    unsafe delegate* unmanaged[Cdecl]<StreamPtr, void> pw_stream_destroy;
    unsafe delegate* unmanaged[Cdecl]<StreamPtr, PwBuffer*> pw_stream_dequeue_buffer;
    unsafe delegate* unmanaged[Cdecl]<StreamPtr, PwBuffer*, int> pw_stream_queue_buffer;
    unsafe delegate* unmanaged[Cdecl]<StreamPtr, bool, int> pw_stream_set_active;

    unsafe delegate* unmanaged[Cdecl]<StreamPtr, SpaHookPtr, StreamEvents*, GCHandle, void>
        pw_stream_add_listener;

    readonly unsafe delegate* unmanaged[Cdecl]<StrPtr, StrPtr, NullPtr, PropertiesPtr>
        pw_properties_new;

    readonly unsafe delegate* unmanaged[Cdecl]<PropertiesPtr, StrPtr, StrPtr, int>
        pw_properties_set;


    readonly unsafe delegate*unmanaged[Cdecl]<MainLoopPtr, LoopPtr> pw_main_loop_get_loop;

    readonly unsafe delegate*unmanaged[Cdecl]<MainLoopPtr, int> pw_main_loop_run;


    // ReSharper restore InconsistentNaming
}