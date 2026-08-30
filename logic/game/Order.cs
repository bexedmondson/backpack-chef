using System.Collections.Generic;

public class Order
{
    public Recipe recipe { get; private set;  }

    private List<OrderStep> orderSteps = new();
    public OrderStep[] steps => orderSteps.ToArray();
    
    public OrderStep currentStep { get; private set; }
    
    private float timeRemaining;

    private OrderState state = OrderState.WaitingToStart;

    public void SetRecipe(Recipe r)
    {
        this.recipe = r;

        foreach (var recipeStep in recipe.steps)
        {
            var orderStep = new OrderStep(recipeStep);
            orderSteps.Add(orderStep);
        }
        
        currentStep = orderSteps[0];
        currentStep.OnStepFinished += OnCurrentStepFinished;
        currentStep.OnStepFailed += OnCurrentStepFailed;
    }

    public bool CanMoveToNextStep()
    {
        var state = GetState();
        return state is OrderState.WaitingToProgress or OrderState.WaitingToStart;
    }

    public void MoveToNextStep()
    {
        currentStep.OnStepFinished -= OnCurrentStepFinished;
        currentStep.OnStepFailed -= OnCurrentStepFailed;
        currentStep = GetNextStep();
        currentStep.OnStepFinished += OnCurrentStepFinished;
        currentStep.OnStepFailed += OnCurrentStepFailed;
        this.state = OrderState.InProgress;
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

        this.state = OrderState.Failed;
        
        Injection.Get<EquipmentDisplayController>().RefreshEquipmentDisplay(equipment);
    }

    public OrderStep GetNextStep()
    {
        if (state == OrderState.WaitingToStart)
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
        state = OrderState.FailedBinned;
    }

    public OrderState GetState()
    {
        if (state == OrderState.Failed || state == OrderState.Complete || state == OrderState.FailedBinned)
            return state;

        RefreshState();
        return state;
    }

    private void RefreshState()
    {
        var currentStepState = GetCurrentStepState();
        state = currentStepState switch {
            OrderStepState.FinishedFailed => OrderState.Failed,
            
            OrderStepState.None when currentStep == steps[0] => OrderState.WaitingToStart,
            
            OrderStepState.InProgress => OrderState.InProgress,
            
            OrderStepState.Finished when currentStep == steps[^1] => OrderState.Complete,
            
            OrderStepState.Finished => OrderState.WaitingToProgress,
            
            _ => state
        };
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
