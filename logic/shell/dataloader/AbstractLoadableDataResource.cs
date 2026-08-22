using Godot;

public abstract partial class AbstractLoadableDataResource : Resource
{
    public virtual void PostLoadSetup() { }
}