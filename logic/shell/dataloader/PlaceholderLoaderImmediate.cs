using Godot;

public partial class PlaceholderLoaderImmediate : Node, IInjectable
{
    [Export]
    private Node placeholderNode;

    private string loadPath;

    protected Node instance;

    public override void _Ready()
    {
        base._Ready();
        if (!placeholderNode.GetSceneInstanceLoadPlaceholder())
        {
            SetProcess(false);
            return;
        }

        loadPath = (placeholderNode as InstancePlaceholder).GetInstancePath();
        
        SetProcess(true);
        ResourceLoader.LoadThreadedRequest(loadPath);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (ResourceLoader.LoadThreadedGetStatus(loadPath) != ResourceLoader.ThreadLoadStatus.Loaded)
            return;
        
        SetProcess(false);
        instance = (placeholderNode as InstancePlaceholder).CreateInstance();
    }
}
