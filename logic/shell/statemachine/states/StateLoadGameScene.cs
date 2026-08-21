using Godot;

public class StateLoadGameScene : AbstractStateLoadScene
{
    public override string Name => nameof(StateLoadGameScene);
    
    protected override string GetScenePath()
    {
        var initialGameSceneReference = Injection.Get<SceneTreeAccessor>().initialGameSceneReference;
        return ResourceUid.UidToPath(initialGameSceneReference.initialGameSceneUid);
    }
}
