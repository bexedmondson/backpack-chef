using System.Threading.Tasks;
using Godot;

public class StateGameRun : AbstractState
{
    public override string Name => nameof(StateGameRun);
    
    protected override async Task<bool> DoStateTasksAsync()
    {
        Injection.Get<EventDispatcher>().Add<RequestExitGameEvent>(OnRequestExit);

        return false;
    }

    private void OnRequestExit(RequestExitGameEvent _)
    {
        Injection.Get<EventDispatcher>().Remove<RequestExitGameEvent>(OnRequestExit);
        
        EndState();
    }
}