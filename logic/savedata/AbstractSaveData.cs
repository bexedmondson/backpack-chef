using Godot;

public abstract partial class AbstractSaveData : Resource
{
    public abstract string fileName { get; }

    public bool changed { get; private set; }

    public void StartChangeListen()
    {
        this.Changed += () => changed = true;
    }

    public void Save()
    {
        changed = false;
    }
}
