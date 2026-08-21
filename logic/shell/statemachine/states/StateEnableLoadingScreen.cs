using System.Threading.Tasks;

public class StateEnableLoadingScreen : AbstractState
{
    public override string Name => nameof(StateEnableLoadingScreen);

    protected override async Task<bool> DoStateTasksAsync()
    {
        var loadingScreenLayer = Injection.Get<SceneTreeAccessor>().loadingScreenLayer;

        loadingScreenLayer.Visible = true;
        loadingScreenLayer.SetProcess(true);
        
        return true;
    }
}