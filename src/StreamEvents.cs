namespace PipeWire;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
unsafe struct StreamEvents {
    public required StreamEventsVersion Version;

    // void(* 	destroy )(void *data)
    public delegate* unmanaged[Cdecl]<GCHandle, void> Destroy;

    // void(* 	state_changed )(void *data, enum pw_stream_state old, enum pw_stream_state state, const char *error)
    // when the stream state changes
    public delegate* unmanaged[Cdecl]<GCHandle, StreamState, StreamState, byte*, void> StateChanged;

    // void(* 	control_info )(void *data, uint32_t id, const struct pw_stream_control *control)
    // Notify information about a control.
    public delegate* unmanaged[Cdecl]<GCHandle, uint, StreamControl*, void> ControlInfo;

    // void(* 	io_changed )(void *data, uint32_t id, void *area, uint32_t size)
    //     when io changed on the stream.
    public delegate* unmanaged[Cdecl]<GCHandle, uint, void*, uint, void> IoChanged;

    // void(* 	param_changed )(void *data, uint32_t id, const struct spa_pod *param)
    // when a parameter changed
    public delegate* unmanaged[Cdecl]<GCHandle, SpaParam, SpaPodPtr, void> ParamChanged;

    // void(* 	add_buffer )(void *data, struct pw_buffer *buffer)
    // when a new buffer was created for this stream
    public delegate* unmanaged[Cdecl]<GCHandle, PwBuffer*, void> AddBuffer;

    // void(* 	remove_buffer )(void *data, struct pw_buffer *buffer)
    // when a buffer was destroyed for this stream
    public delegate* unmanaged[Cdecl]<GCHandle, PwBuffer*, void> RemoveBuffer;

    // void(* 	process )(void *data)
    //     when a buffer can be queued (for playback streams) or dequeued (for capture streams).
    public delegate* unmanaged[Cdecl]<GCHandle, void> Process;

    // void(* 	drained )(void *data)
    //     The stream is drained.
    public delegate* unmanaged[Cdecl]<GCHandle, void> Drained;

    // void(* 	command )(void *data, const struct spa_command *command)
    // A command notify, Since 0.3.39:1.
    public delegate* unmanaged[Cdecl]<GCHandle, SpaCommandPtr, void> Command;

    // void(* 	trigger_done )(void *data)
    //     a trigger_process completed. 
    public delegate* unmanaged[Cdecl]<GCHandle, void> TriggerDone;
}

enum StreamEventsVersion {
    STREAM_EVENTS = 2,
}

unsafe struct StreamControl {
    public byte* Name;
    public Flag Flags;
    public float Def;
    public float Min;
    public float Max;
    public float* Values;
    public uint NValues;
    public uint MaxValues;

    public enum Flag: uint { }
}