using Godot;

[Tool]
[Icon("Icons/reorderable_vbox_icon.svg")]
[GlobalClass]
public partial class ReorderableVBox : ReorderableContainer
{
    protected void _Ready()
    {
        CustomMinimumSize = new Vector2(0, CustomMinimumSize.Y);
        base._Ready();
    }
}