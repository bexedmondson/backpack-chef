using System.Threading.Tasks;

public class StateDisableLoadingScreen : AbstractState
{
    public override string Name => nameof(StateDisableLoadingScreen);

    protected override async Task<bool> DoStateTasksAsync()
    {
        var loadingScreenLayer = Injection.Get<SceneTreeAccessor>().loadingScreenLayer;

        loadingScreenLayer.Visible = false;
        loadingScreenLayer.SetProcess(false);
        
        return true;
    }
}