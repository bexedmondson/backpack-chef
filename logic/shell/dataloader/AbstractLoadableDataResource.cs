using Godot;

public abstract partial class AbstractLoadableDataResource : Resource
{
    public abstract void PostLoadSetup();
}