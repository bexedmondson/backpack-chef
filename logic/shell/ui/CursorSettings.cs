
using Godot;

[GlobalClass]
public partial class CursorSettings : Resource
{
    [Export]
    public AtlasTexture texture;
    
    [Export]
    public AtlasTexture largeTexture;

    [Export]
    public Vector2I hotspot = Vector2I.Zero;
}
