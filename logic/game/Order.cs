using System.Collections.Generic;

public class Order(Recipe recipe)
{
    public Recipe recipe { get; private set; } = recipe;

    private List<OrderStep> orderSteps = new();
    public OrderStep[] steps => orderSteps.ToArray();
    
    public OrderStep currentStep { get; private set; }

    private double initialTimeRemaining;
    private double timeRemaining;

    private OrderState state = OrderState.WaitingToStart;

    private Stack<OrderState> stateHistory = new( [ OrderState.WaitingToStart ] );

    public void Initialise()
    {
        initialTimeRemaining = recipe.defaultTimeLimit;
        timeRemaining = recipe.defaultTimeLimit;

        foreach (var recipeStep in recipe.steps)
        {
            var orderStep = new OrderStep(recipeStep);
            orderSteps.Add(orderStep);
        }
        
        currentStep = orderSteps[0];
        currentStep.OnStepFinished += OnCurrentStepFinished;
        currentStep.OnStepFailed += OnCurrentStepFailed;
    }
    
    public void MoveToNextStep()
    {
        currentStep.OnStepFinished -= OnCurrentStepFinished;
        currentStep.OnStepFailed -= OnCurrentStepFailed;
        currentStep = GetNextStep();
        currentStep.OnStepFinished += OnCurrentStepFinished;
        currentStep.OnStepFailed += OnCurrentStepFailed;
        SetState(OrderState.InProgress);
    }

    private void OnCurrentStepFinished()
    {
        var isAtEquipment = Injection.Get<EquipmentManager>().TryGetEquipmentWithOrder(this, out var equipment);
        if (!isAtEquipment)
            return;
        
        Injection.Get<EquipmentDisplayController>().RefreshEquipmentDisplay(equipment);
    }

    private void OnCurrentStepFailed()
    {
        var isAtEquipment = Injection.Get<EquipmentManager>().TryGetEquipmentWithOrder(this, out var equipment);
        if (!isAtEquipment)
            return;

        SetState(OrderState.FailedStep);
        
        Injection.Get<EquipmentDisplayController>().RefreshEquipmentDisplay(equipment);
    }

    public OrderStep GetNextStep()
    {
        if (GetState() == OrderState.WaitingToStart)
            return currentStep;
        
        int currentStepIndex = orderSteps.IndexOf(currentStep);
        int nextStepIndex = currentStepIndex + 1;
        if (nextStepIndex > (recipe.steps.Count - 1))
        {
            Log.Error($"Next step index is {nextStepIndex}, order step count is {orderSteps.Count}! Returning null.", true);
            return null;
        }
        
        return orderSteps[nextStepIndex];
    }

    public void Bin()
    {
        SetState(OrderState.FailedBinned);
    }

    public OrderState GetState()
    {
        if (state.IsEnded())
            return state;

        RefreshState();
        return state;
    }

    public bool HasMadeAnyProgress()
    {
        return stateHistory.Contains(OrderState.InProgress);
    }

    private void SetState(OrderState newState)
    {
        if (state == newState)
            return;
        
        Log.Print($"Order {recipe.name} moving from state {state} to state {newState}");
        state = newState;
        if (stateHistory.TryPeek(out var prevState) && prevState != newState)
            stateHistory.Push(newState);

        string history = "";
        foreach (var orderState in stateHistory)
        {
            history += orderState;
        }
        Log.Print($"Order {recipe.name} state history now {history}");
    }

    public double GetTimeRemainingProportion()
    {
        return timeRemaining / initialTimeRemaining;
    }

    private void RefreshState()
    {
        var currentStepState = GetCurrentStepState();
        
        //only need to update state if not ended already, so return early here
        if (state.IsEnded())
            return;

        SetState(currentStepState switch {
            OrderStepState.FinishedFailed => OrderState.FailedStep,

            OrderStepState.None when currentStep == steps[0] => OrderState.WaitingToStart,

            OrderStepState.InProgress => OrderState.InProgress,

            OrderStepState.Finished when currentStep == steps[^1] => OrderState.Complete,

            OrderStepState.Finished => OrderState.WaitingToProgress,

            _ => state
        });
    }

    public void DecreaseTimeRemaining(double delta)
    {
        timeRemaining -= delta;

        if (timeRemaining <= 0)
            SetState(OrderState.FailedExpired);
    }

    public OrderStepState GetCurrentStepState()
    {
        return currentStep switch {
            { didStepFail: true } => OrderStepState.FinishedFailed,
            { isStepFinished: true } => OrderStepState.Finished,
            { isStepInProgress: true } => OrderStepState.InProgress,
            _ => OrderStepState.None
        };
    }
}
