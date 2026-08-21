using Godot;

//autoloaded
public partial class SceneTreeAccessor : Node, IInjectable
{
    public SceneTree currentSceneTree => GetTree();

    public LoadingScreenLayer loadingScreenLayer { get; private set; }
    
    public InitialGameSceneReference initialGameSceneReference { get; private set; }

    public Node activeSceneNode { get; set; }

    public override void _EnterTree()
    {
        base._EnterTree();
        Injection.Register(this);

        loadingScreenLayer = GetNode<LoadingScreenLayer>("/root/Shell/LoadingCanvasLayer");
        initialGameSceneReference = GetNode<InitialGameSceneReference>("/root/Shell/InitialGameSceneReference");
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Injection.Deregister(this);
    }
}