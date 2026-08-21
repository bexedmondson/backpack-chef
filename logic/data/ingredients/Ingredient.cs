using Godot;

[GlobalClass, Icon("res://assets/editor/icons/apple.svg")]
public partial class Ingredient : Resource
{
    [Export]
    public string name { get; private set; }

    [Export]
    public Texture icon;
}
