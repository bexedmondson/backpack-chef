using Godot;

public partial class InitialGameSceneReference : Node
{
    [Export]
    public string initialSaveSelectSceneUid { get; private set; }
    
    [Export]
    public string initialGameSceneUid { get; private set; }
}
