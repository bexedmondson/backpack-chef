
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public abstract class AbstractState
{
    public event Action<AbstractState> OnFinished;

    public abstract string Name { get; }

    private AbstractState defaultNextState;

    private List<AbstractStateTransition> nextStateTransitionAlternatives = new();

    public void AddDefaultStateTransition(AbstractState defaultState)
    {
        defaultNextState = defaultState;
    }

    public void AddAlternativeStateTransition(AbstractStateTransition stateTransition)
    {
        nextStateTransitionAlternatives.Add(stateTransition);
    }

    public async void Run()
    {
        await StateTasksAsync();
    }

    private async Task StateTasksAsync()
    {
        Log.Print($"State tasks begin: {Name}", Colors.Aqua);

        bool shouldEndStateImmediately = await DoStateTasksAsync();

        Log.Print($"State tasks end: {Name}, should end state: {shouldEndStateImmediately}", Colors.LightBlue);
        
        if (shouldEndStateImmediately)
            EndState();
    }

    protected virtual async Task<bool> DoStateTasksAsync()
    {
        return false;
    }

    protected void EndState()
    {
        Log.Print($"{Name}", Colors.Aqua, true);
        OnFinished?.Invoke(GetNextState());
    }

    protected AbstractState GetNextState()
    {
        foreach (var stateTransition in nextStateTransitionAlternatives)
        {
            if (stateTransition.EvaluateShouldTransition(this))
                return stateTransition.targetState;
        }

        return defaultNextState;
    }
}