public abstract class AbstractStateTransition
{
    public AbstractState targetState { get; private set; }

    public AbstractStateTransition(AbstractState targetState)
    {
        this.targetState = targetState;
    }

    public abstract bool EvaluateShouldTransition(AbstractState fromState);
}