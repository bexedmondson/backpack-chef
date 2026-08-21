using System.Threading.Tasks;

public class StateInitialiseShellSystems : AbstractState
{
    public override string Name => nameof(StateInitialiseShellSystems);

    protected override async Task<bool> DoStateTasksAsync()
    {
        EventDispatcher eventDispatcher = new EventDispatcher();
        
        DataLoader dataLoader = new DataLoader();

        GameSceneManager gameSceneManager = new GameSceneManager();

        SaveManager saveManager = new SaveManager();
        
        var loadingScreenLayer = Injection.Get<SceneTreeAccessor>().loadingScreenLayer;
        loadingScreenLayer.StartListeningForProgress();

        return true;
    }
}
