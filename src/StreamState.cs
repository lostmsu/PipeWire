namespace PipeWire;

public enum StreamState {
    ERROR = -1,
    UNCONNECTED = 0,
    CONNECTING = 1,
    PAUSED = 2,
    STREAMING = 3,
}