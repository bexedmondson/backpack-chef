using Godot;

public class StateLoadSaveSelectScene : AbstractStateLoadScene
{
    public override string Name => nameof(StateLoadSaveSelectScene);
    
    protected override string GetScenePath()
    {
        var initialGameSceneReference = Injection.Get<SceneTreeAccessor>().initialGameSceneReference;
        return ResourceUid.UidToPath(initialGameSceneReference.initialSaveSelectSceneUid);
    }
}
