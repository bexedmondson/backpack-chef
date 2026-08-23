using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public class StateGameSetup : AbstractState
{
    public override string Name => nameof(StateGameSetup);

    protected override async Task<bool> DoStateTasksAsync()
    {
        RNG.Load();
        
        GameManagerContainer managerContainer = new GameManagerContainer();
        
        var managers = new List<AbstractManager>(){
            new KitchenManager(),
            new OrderManager(),
            new RecipeAvailabilityManager(),
            new BackpackManager()
        };

        foreach (var manager in managers)
        {
            managerContainer.RegisterManager(manager);
            manager.LoadFromSaveFile();
        }

        foreach (var manager in managers)
        {
            manager.Setup();
        }
        
        return true;
    }
}