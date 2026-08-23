using Godot;

public partial class SettingsPanel : Control
{
    [Export]
    public Texture bigCursor;
    
    public void ToggleBigCursor(bool on)
    {
        Injection.Get<CustomCursorSetup>().SetForceLarge(on);
    }

    public void ToggleFullscreen(bool on)
    {
        DisplayServer.WindowSetMode(on ? DisplayServer.WindowMode.ExclusiveFullscreen : DisplayServer.WindowMode.Windowed);
    }
}
