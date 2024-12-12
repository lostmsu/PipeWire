namespace PipeWire;

public sealed class PipeWireStream: IDisposable {
    public StreamPtr Handle { get; private set; }
    readonly PipeWireLib pw;

    public void Activate() {
        this.EnsureNotDisposed();
        this.pw.SetStreamActive(this, true);
    }

    public void Deactivate() {
        this.EnsureNotDisposed();
        this.pw.SetStreamActive(this, false);
    }

    internal PipeWireStream(PipeWireLib pw, StreamPtr handle) {
        if (handle == default)
            throw new ArgumentNullException(nameof(handle));

        this.pw = pw ?? throw new ArgumentNullException(nameof(pw));
        this.Handle = handle;
    }

    void EnsureNotDisposed() {
        if (this.Handle == default)
            throw new ObjectDisposedException(nameof(PipeWireStream));
    }

    void ReleaseUnmanagedResources() {
        lock (this.pw) {
            this.pw.DestroyStream(this.Handle);
            this.Handle = default;
        }
    }

    public void Dispose() {
        this.ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~PipeWireStream() {
        this.ReleaseUnmanagedResources();
    }
}