using Godot;

public partial class CustomCursorSetup : Node
{
    [Export]
    private Godot.Collections.Dictionary<Input.CursorShape, AtlasTexture> cursorShapeMap = new();
    
    
    public override void _Ready()
    {
        foreach (var kvp in cursorShapeMap)
        {
            Input.SetCustomMouseCursor(kvp.Value, kvp.Key);
        }
    }
}