namespace PipeWire;

public static class PropertyKey
{
    // Security and Protocol
    public const string PROTOCOL = "pipewire.protocol";
    public const string ACCESS = "pipewire.access";
    public const string CLIENT_ACCESS = "pipewire.client.access";
    public const string SEC_PID = "pipewire.sec.pid";
    public const string SEC_UID = "pipewire.sec.uid";
    public const string SEC_GID = "pipewire.sec.gid";
    public const string SEC_LABEL = "pipewire.sec.label";
    public const string SEC_SOCKET = "pipewire.sec.socket";
    public const string SEC_ENGINE = "pipewire.sec.engine";
    public const string SEC_APP_ID = "pipewire.sec.app-id";
    public const string SEC_INSTANCE_ID = "pipewire.sec.instance-id";

    // Library Names
    public const string LIBRARY_NAME_SYSTEM = "library.name.system";
    public const string LIBRARY_NAME_LOOP = "library.name.loop";
    public const string LIBRARY_NAME_DBUS = "library.name.dbus";

    // Object Properties
    public const string OBJECT_PATH = "object.path";
    public const string OBJECT_ID = "object.id";
    public const string OBJECT_SERIAL = "object.serial";
    public const string OBJECT_LINGER = "object.linger";
    public const string OBJECT_REGISTER = "object.register";
    public const string OBJECT_EXPORT = "object.export";

    // Config Properties
    public const string CONFIG_PREFIX = "config.prefix";
    public const string CONFIG_NAME = "config.name";
    public const string CONFIG_OVERRIDE_PREFIX = "config.override.prefix";
    public const string CONFIG_OVERRIDE_NAME = "config.override.name";

    // Loop Properties
    public const string LOOP_NAME = "loop.name";
    public const string LOOP_CLASS = "loop.class";
    public const string LOOP_RT_PRIO = "loop.rt-prio";
    public const string LOOP_CANCEL = "loop.cancel";

    // Context Properties
    public const string CONTEXT_PROFILE_MODULES = "context.profile.modules";
    public const string USER_NAME = "context.user-name";
    public const string HOST_NAME = "context.host-name";

    // Core Properties
    public const string CORE_NAME = "core.name";
    public const string CORE_VERSION = "core.version";
    public const string CORE_DAEMON = "core.daemon";
    public const string CORE_ID = "core.id";
    public const string CORE_MONITORS = "core.monitors";

    // CPU Properties
    public const string CPU_MAX_ALIGN = "cpu.max-align";
    public const string CPU_CORES = "cpu.cores";

    // Priority Properties
    public const string PRIORITY_SESSION = "priority.session";
    public const string PRIORITY_DRIVER = "priority.driver";

    // Remote Properties
    public const string REMOTE_NAME = "remote.name";
    public const string REMOTE_INTENTION = "remote.intention";

    // Application Properties
    public const string APP_NAME = "application.name";
    public const string APP_ID = "application.id";
    public const string APP_VERSION = "application.version";
    public const string APP_ICON = "application.icon";
    public const string APP_ICON_NAME = "application.icon-name";
    public const string APP_LANGUAGE = "application.language";
    public const string APP_PROCESS_ID = "application.process.id";
    public const string APP_PROCESS_BINARY = "application.process.binary";
    public const string APP_PROCESS_USER = "application.process.user";
    public const string APP_PROCESS_HOST = "application.process.host";
    public const string APP_PROCESS_MACHINE_ID = "application.process.machine-id";
    public const string APP_PROCESS_SESSION_ID = "application.process.session-id";

    // Window Properties
    public const string WINDOW_X11_DISPLAY = "window.x11.display";

    // Client Properties
    public const string CLIENT_ID = "client.id";
    public const string CLIENT_NAME = "client.name";
    public const string CLIENT_API = "client.api";

    // Node Properties
    public const string NODE_ID = "node.id";
    public const string NODE_NAME = "node.name";
    public const string NODE_NICK = "node.nick";
    public const string NODE_DESCRIPTION = "node.description";
    public const string NODE_PLUGGED = "node.plugged";
    public const string NODE_SESSION = "node.session";
    public const string NODE_GROUP = "node.group";
    public const string NODE_LATENCY = "node.latency";
    public const string NODE_MAX_LATENCY = "node.max-latency";
    public const string NODE_RATE = "node.rate";

    // Port Properties
    public const string PORT_ID = "port.id";
    public const string PORT_NAME = "port.name";
    public const string PORT_DIRECTION = "port.direction";
    public const string PORT_ALIAS = "port.alias";
    public const string PORT_PHYSICAL = "port.physical";

    // Link Properties
    public const string LINK_ID = "link.id";
    public const string LINK_INPUT_NODE = "link.input.node";
    public const string LINK_INPUT_PORT = "link.input.port";
    public const string LINK_OUTPUT_NODE = "link.output.node";
    public const string LINK_OUTPUT_PORT = "link.output.port";

    // Device Properties
    public const string DEVICE_ID = "device.id";
    public const string DEVICE_NAME = "device.name";
    public const string DEVICE_NICK = "device.nick";
    public const string DEVICE_STRING = "device.string";
    public const string DEVICE_API = "device.api";
    public const string DEVICE_DESCRIPTION = "device.description";

    // Media Properties
    public const string MEDIA_TYPE = "media.type";
    public const string MEDIA_CATEGORY = "media.category";
    public const string MEDIA_ROLE = "media.role";
    public const string MEDIA_CLASS = "media.class";
    public const string MEDIA_NAME = "media.name";
    public const string MEDIA_TITLE = "media.title";
    public const string MEDIA_ARTIST = "media.artist";
    public const string MEDIA_ALBUM = "media.album";
    public const string MEDIA_FORMAT = "media.format";

    // Audio Properties
    public const string AUDIO_CHANNEL = "audio.channel";
    public const string AUDIO_RATE = "audio.rate";
    public const string AUDIO_CHANNELS = "audio.channels";
    public const string AUDIO_FORMAT = "audio.format";
    public const string AUDIO_ALLOWED_RATES = "audio.allowed-rates";

    // Video Properties
    public const string VIDEO_RATE = "video.framerate";
    public const string VIDEO_FORMAT = "video.format";
    public const string VIDEO_SIZE = "video.size";
}