public class StateTransitionShouldRunGame : AbstractStateTransition
{
    public StateTransitionShouldRunGame(StateGameRun targetState) : base(targetState)
    {
    }
    
    public override bool EvaluateShouldTransition(AbstractState fromState)
    {
        var gameSceneManager = Injection.Get<GameSceneManager>();
        if (gameSceneManager == null)
            return false;
        
        return gameSceneManager.HasActiveGameSceneNodeInTree;
    }
}
