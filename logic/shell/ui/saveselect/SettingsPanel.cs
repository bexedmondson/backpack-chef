using Godot;

public partial class SettingsPanel : Control
{
    [Export]
    public Texture bigCursor;
    
    public void ToggleBigCursor(bool on)
    {
        Input.SetCustomMouseCursor(on ? bigCursor : null);
    }

    public void ToggleFullscreen(bool on)
    {
        DisplayServer.WindowSetMode(on ? DisplayServer.WindowMode.ExclusiveFullscreen : DisplayServer.WindowMode.Windowed);
    }
}
