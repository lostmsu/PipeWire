namespace PipeWire;

public record struct StreamPtr(IntPtr Pointer);

public record struct NullPtr(IntPtr Pointer);

public record struct LoopPtr(IntPtr Pointer);
public record struct MainLoopPtr(IntPtr Pointer);

public record struct SpaCommandPtr(IntPtr Pointer);

public record struct SpaHookPtr(IntPtr Pointer);

public record struct SpaPodPtr(IntPtr Pointer);

public record struct PropertiesPtr(IntPtr Pointer);