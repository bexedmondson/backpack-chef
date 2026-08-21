using System.Threading.Tasks;

public class StateSaveSelectRun : AbstractState
{
    public override string Name => nameof(StateSaveSelectRun);
    
    protected override async Task<bool> DoStateTasksAsync()
    {
        Injection.Get<EventDispatcher>().Add<SaveSelectedEvent>(OnRequestExit);

        return false;
    }

    private void OnRequestExit(SaveSelectedEvent _)
    {
        Injection.Get<EventDispatcher>().Remove<SaveSelectedEvent>(OnRequestExit);
        
        EndState();
    }
}