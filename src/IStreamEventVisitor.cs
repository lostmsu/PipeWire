namespace PipeWire;

public interface IStreamEventVisitor {
    void OnDestroy();
    void OnDrained();
    void OnParamChanged(SpaParam param, SpaPodPtr pod);
    void OnProcess();
    void OnStateChanged(StreamState old, StreamState state, string? error);
}