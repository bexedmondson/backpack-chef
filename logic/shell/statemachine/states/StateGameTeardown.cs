using System.Threading.Tasks;
using Godot;

public class StateGameTeardown : AbstractState
{
    public override string Name => nameof(StateGameTeardown);

    protected override async Task<bool> DoStateTasksAsync()
    {
        var gameManagerContainer = Injection.Get<GameManagerContainer>();
        var managers = gameManagerContainer.GetManagers();

        foreach (var manager in managers)
        {
            manager.Cleanup();
            gameManagerContainer.DeregisterManager(manager);
        }
        
        Injection.Deregister(gameManagerContainer);
        
        return true;
    }
}