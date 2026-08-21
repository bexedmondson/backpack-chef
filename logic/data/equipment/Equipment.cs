using Godot;

[GlobalClass, Icon("res://assets/editor/icons/microwave.svg")]
public abstract partial class Equipment : Resource
{
    [Export]
    public string name { get; private set; }

    [Export]
    public Texture icon;
}
