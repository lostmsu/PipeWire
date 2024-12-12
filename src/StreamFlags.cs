namespace PipeWire;

/// <summary>
/// Flags controlling the behavior of a PipeWire stream.
/// </summary>
[Flags]
public enum StreamFlags {
    /// <summary>
    /// No flags.
    /// </summary>
    NONE = 0,

    /// <summary>
    /// Try to automatically connect this stream.
    /// </summary>
    AUTO_CONNECT = 1 << 0,

    /// <summary>
    /// Start the stream inactive, pw_stream_set_active() needs to be called explicitly.
    /// </summary>
    INACTIVE = 1 << 1,

    /// <summary>
    /// Mmap the buffers except DmaBuf that is not explicitly marked as mappable.
    /// </summary>
    MAP_BUFFERS = 1 << 2,

    /// <summary>
    /// Be a driver.
    /// </summary>
    DRIVER = 1 << 3,

    /// <summary>
    /// Call process from the realtime thread. You MUST use RT safe functions in the process callback.
    /// </summary>
    RT_PROCESS = 1 << 4,

    /// <summary>
    /// Don't convert format.
    /// </summary>
    NO_CONVERT = 1 << 5,

    /// <summary>
    /// Require exclusive access to the device.
    /// </summary>
    EXCLUSIVE = 1 << 6,

    /// <summary>
    /// Don't try to reconnect this stream when the sink/source is removed.
    /// </summary>
    DONT_RECONNECT = 1 << 7,

    /// <summary>
    /// The application will allocate buffer memory. In the add_buffer event, the data of the buffer should be set.
    /// </summary>
    ALLOC_BUFFERS = 1 << 8,

    /// <summary>
    /// The output stream will not be scheduled automatically but _trigger_process() needs to be called.
    /// This can be used when the output of the stream depends on input from other streams.
    /// </summary>
    TRIGGER = 1 << 9,

    /// <summary>
    /// Buffers will not be dequeued/queued from the realtime process() function.
    /// This is assumed when RT_PROCESS is unset but can also be the case when the process() function
    /// does a trigger_process() that will then dequeue/queue a buffer from another process() function.
    /// Available since 0.3.73.
    /// </summary>
    ASYNC = 1 << 10,

    /// <summary>
    /// Call process as soon as there is a buffer to dequeue.
    /// This is only relevant for playback and when not using RT_PROCESS.
    /// It can be used to keep the maximum number of buffers queued.
    /// Available since 0.3.81.
    /// </summary>
    EARLY_PROCESS = 1 << 11
}