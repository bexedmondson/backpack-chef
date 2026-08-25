using Godot;

[GlobalClass, Icon("res://assets/editor/icons/microwave.svg")]
public abstract partial class Equipment : AbstractLoadableDataResource
{
    [Export]
    public string name { get; private set; }

    [Export]
    public Texture2D icon;
    
    [Export]
    public Texture2D smallIcon;
}
