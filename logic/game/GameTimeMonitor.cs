using System;
using Godot;

public partial class GameTimeMonitor : Node
{
    public Action<double> OnProcess;

    public override void _Ready()
    {
        base._Ready();
        Injection.Get<OrderManager>().OnGameStart(this);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        OnProcess.Invoke(delta);
    }
}
