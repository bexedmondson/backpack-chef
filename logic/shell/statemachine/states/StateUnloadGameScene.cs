using System.Threading.Tasks;
using Godot;

public class StateUnloadGameScene : AbstractState
{
    public override string Name => nameof(StateUnloadGameScene);

    protected override async Task<bool> DoStateTasksAsync()
    {
        Callable.From(Unload).CallDeferred();
        return false;
    }
    
    private async void Unload()
    {
        await Injection.Get<GameSceneManager>().RemoveActiveSceneNode();

        EndState();
    }
}