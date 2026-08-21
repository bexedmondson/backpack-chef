using Godot;
using System;

public partial class Shell : Node
{
    private StateMachine stateMachine;

    public override void _Process(double delta)
    {
        stateMachine = new StateMachine();
        stateMachine.Begin();
        this.SetProcess(false);
    }
}