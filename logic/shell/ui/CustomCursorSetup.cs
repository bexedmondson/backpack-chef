using Godot;

public partial class CustomCursorSetup : Node, IInjectable
{
    [Export]
    private Godot.Collections.Dictionary<Input.CursorShape, CursorSettings> cursorShapeMap = new();

    private bool forceLarge = false;

    public override void _EnterTree()
    {
        base._EnterTree();
        Injection.Register(this);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Injection.Deregister(this);
    }

    public override void _Ready()
    {
        ResizeCursor();
        GetViewport().SizeChanged += ResizeCursor;
    }

    public void SetForceLarge(bool force)
    {
        forceLarge = force;
        ResizeCursor();
    }
    
    private void ResizeCursor()
    {
        bool useLargeCursor = forceLarge;

        if (!forceLarge)
        {
            Vector2I windowSize = GetWindow().Size;
            int scale = 1;
            if (windowSize.X < windowSize.Y)
                scale = windowSize.X / GetWindow().ContentScaleSize.X;
            else
                scale = windowSize.Y / GetWindow().ContentScaleSize.Y;
            Log.Print(scale.ToString(), Colors.Green);
            if (scale >= 3)
                useLargeCursor = true;
        }
        
        foreach (var kvp in cursorShapeMap)
        {
            var texture = useLargeCursor ? kvp.Value.largeTexture : kvp.Value.texture; 
            //magic number of 3 comes from the fact that the atlas for large cursors is 3x bigger than the atlas for default cursors
            Input.SetCustomMouseCursor(texture, kvp.Key, kvp.Value.hotspot * (useLargeCursor ? 3 : 1));
        }
    }
}