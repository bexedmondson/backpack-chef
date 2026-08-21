using System.Threading.Tasks;
using Godot;

public class StateLoadSaveData : AbstractState
{
    public override string Name => nameof(StateLoadSaveData);

    protected override async Task<bool> DoStateTasksAsync()
    {
        await Injection.Get<SaveManager>().LoadSelectedSave();
        
        return true;
    }
}