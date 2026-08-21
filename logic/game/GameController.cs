using System;
using System.Collections.Generic;
using Godot;

public partial class GameController : Node, IInjectable
{
    private GameManagerContainer gameManagerContainer;
    private EventDispatcher eventDispatcher;
    private SaveManager saveManager;
    
    public override void _EnterTree()
    {
        base._EnterTree();
        Injection.Register(this);

        saveManager = Injection.Get<SaveManager>();
        gameManagerContainer = Injection.Get<GameManagerContainer>();
        eventDispatcher = Injection.Get<EventDispatcher>();
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        Injection.Deregister(this);

        saveManager = null;
        gameManagerContainer = null;
        eventDispatcher = null;
    }
}