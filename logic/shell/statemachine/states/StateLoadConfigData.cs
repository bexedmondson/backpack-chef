using System.Collections.Generic;
using System.Threading.Tasks;

public class StateLoadConfigData : AbstractState
{
    public override string Name => nameof(StateLoadConfigData);

    protected override async Task<bool> DoStateTasksAsync()
    {
        var dataLoader = Injection.Get<DataLoader>();

        await dataLoader.LoadAllResources();

        return true;
    }
}