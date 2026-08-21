public class StateTransitionShouldBeginLoadingGame : AbstractStateTransition
{

    public StateTransitionShouldBeginLoadingGame(AbstractState targetState) : base(targetState)
    {
    }

    public override bool EvaluateShouldTransition(AbstractState fromState)
    {
        var gameSceneManager = Injection.Get<GameSceneManager>();
        if (gameSceneManager == null)
            return false;
        
        return gameSceneManager.HasActiveSaveSelectSceneNodeInTree;
    }
}
