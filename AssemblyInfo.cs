using System.Runtime.CompilerServices;

// Rider can't handle 'nameof' generated strings apparently
[assembly:InternalsVisibleTo("LunyScript-Test")]
[assembly:InternalsVisibleTo("LunyScript.Godot")]
[assembly:InternalsVisibleTo("LunyScript.GodotEditor")]
[assembly:InternalsVisibleTo("LunyScript.Unity")]
[assembly:InternalsVisibleTo("LunyScript.Unity.UI")]
[assembly:InternalsVisibleTo("LunyScript.UnityEditor")]

// reserved namespaces for future C# engine implementations
[assembly:InternalsVisibleTo(nameof(LunyScript) + ".Cocos")] // no C# support (yet)
[assembly:InternalsVisibleTo(nameof(LunyScript) + ".CocosEditor")]
[assembly:InternalsVisibleTo(nameof(LunyScript) + ".CryEngine")]
[assembly:InternalsVisibleTo(nameof(LunyScript) + ".CryEngineEditor")]
[assembly:InternalsVisibleTo(nameof(LunyScript) + ".Bevy")] // no C# support (yet)
[assembly:InternalsVisibleTo(nameof(LunyScript) + ".BevyEditor")]
[assembly:InternalsVisibleTo(nameof(LunyScript) + ".Evergine")]
[assembly:InternalsVisibleTo(nameof(LunyScript) + ".EvergineEditor")]
[assembly:InternalsVisibleTo(nameof(LunyScript) + ".Flax")]
[assembly:InternalsVisibleTo(nameof(LunyScript) + ".FlaxEditor")]
[assembly:InternalsVisibleTo(nameof(LunyScript) + ".Open3D")] // no C# support (yet)
[assembly:InternalsVisibleTo(nameof(LunyScript) + ".Open3DEditor")]
[assembly:InternalsVisibleTo(nameof(LunyScript) + ".Stride")]
[assembly:InternalsVisibleTo(nameof(LunyScript) + ".StrideEditor")]
[assembly:InternalsVisibleTo(nameof(LunyScript) + ".Unigine")]
[assembly:InternalsVisibleTo(nameof(LunyScript) + ".UnigineEditor")]
[assembly:InternalsVisibleTo(nameof(LunyScript) + ".Unreal")]
[assembly:InternalsVisibleTo(nameof(LunyScript) + ".UnrealEditor")]
