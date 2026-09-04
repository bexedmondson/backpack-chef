using Godot;

[Tool]
public partial  class ReorderableContainerPlugin : EditorPlugin
{
    public override void _EnterTree()
    {
        base._EnterTree();
        var containerScript = GD.Load<Script>("ReorderableContainer.cs");
        var containerIcon = GD.Load<Texture2D>("Icons/reorderable_container_icon.svg");
        AddCustomType("ReorderableContainer", "Container", containerScript, containerIcon);
        var containerVBoxScript = GD.Load<Script>("ReorderableVBox.cs");
        var containerVBoxIcon = GD.Load<Texture2D>("Icons/reorderable_vbox_icon.svg");
        AddCustomType("ReorderableVBox", "Container", containerVBoxScript, containerVBoxIcon);
        var containerHBoxScript = GD.Load<Script>("ReorderableHBox.cs");
        var containerHBoxIcon = GD.Load<Texture2D>("Icons/reorderable_hbox_icon.svg");
        AddCustomType("ReorderableHBox", "Container", containerHBoxScript, containerHBoxIcon);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        RemoveCustomType("ReorderableContainer");
        RemoveCustomType("ReorderableVBox");
        RemoveCustomType("ReorderableHBox");
    }
}