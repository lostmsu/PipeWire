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

        this.pw_stream_new_simple =
            (delegate* unmanaged[Cdecl]<IntPtr, string, IntPtr, uint, IntPtr, uint, IntPtr, IntPtr>)
            NativeLibrary.GetExport(handle, nameof(this.pw_stream_new_simple));

        this.pw_stream_connect =
            (delegate* unmanaged[Cdecl]<IntPtr, int, uint, uint, IntPtr, uint, int>)
            NativeLibrary.GetExport(handle, nameof(this.pw_stream_connect));

        this.pw_stream_destroy = (delegate* unmanaged[Cdecl]<IntPtr, void>)
            NativeLibrary.GetExport(handle, nameof(this.pw_stream_destroy));
    }

    public unsafe void Init() {
        this.EnsureAvailable();
        this.pw_init(IntPtr.Zero, IntPtr.Zero);
    }

    public unsafe IntPtr CreateStream(string name, uint mode = 0, uint flags = 0) {
        this.EnsureAvailable();
        IntPtr stream = this.pw_stream_new_simple(
            IntPtr.Zero, // Use default loop
            name,
            IntPtr.Zero, // Default properties
            mode,
            IntPtr.Zero, // Default format
            flags,
            IntPtr.Zero // No params
        );

        if (stream == IntPtr.Zero)
            throw new InvalidOperationException("Failed to create PipeWire stream");

        return stream;
    }

    public unsafe bool ConnectStream(IntPtr stream, uint targetId = uint.MaxValue) {
        this.EnsureAvailable();
        const int PW_DIRECTION_INPUT = 0;
        const uint PW_STREAM_FLAG_AUTOCONNECT = 1;

        int result = this.pw_stream_connect(
            stream,
            PW_DIRECTION_INPUT,
            targetId,
            PW_STREAM_FLAG_AUTOCONNECT,
            IntPtr.Zero,
            0
        );

        return result == 0;
    }

    public unsafe void DestroyStream(IntPtr stream) {
        this.EnsureAvailable();
        if (stream != IntPtr.Zero) {
            this.pw_stream_destroy(stream);
        }
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

    unsafe delegate* unmanaged[Cdecl]<IntPtr, string, IntPtr, uint, IntPtr, uint, IntPtr, IntPtr>
        pw_stream_new_simple;

    unsafe delegate* unmanaged[Cdecl]<IntPtr, int, uint, uint, IntPtr, uint, int> pw_stream_connect;

    unsafe delegate* unmanaged[Cdecl]<IntPtr, void> pw_stream_destroy;
    // ReSharper restore InconsistentNaming
}