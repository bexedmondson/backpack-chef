using Godot;

public partial class DebugButton : Button
{
    private double timeSinceFirstClick = 0d;

    private int clickCount = 0;
    
    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustReleased("debug_toggle"))
            ToggleDebug();
    }

    public override void _Process(double delta)
    {
        if (clickCount == 0)
            return;
        
        timeSinceFirstClick += delta;

        if (timeSinceFirstClick > 1)
        {
            clickCount = 0;
            timeSinceFirstClick = 0;
        }
    }

    public void OnClick()
    {
        clickCount++;
        Log.Print(clickCount.ToString());

        if (clickCount >= 3)
        {
            timeSinceFirstClick = 0;
            clickCount = 0;
            ToggleDebug();
        }
    }

    private void ToggleDebug()
    {
        GameDebug.On = !GameDebug.On;
    }
}