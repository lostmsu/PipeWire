namespace PipeWire;

using System.IO;
using System.Runtime.InteropServices;

using Nix;

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
    }

    public unsafe void Init() {
        this.EnsureAvailable();
        this.pw_init(IntPtr.Zero, IntPtr.Zero);
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
    // ReSharper restore InconsistentNaming
}