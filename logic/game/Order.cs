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
    }

    public void MoveToNextStep()
    {
        currentStep = GetNextStep();
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

    public OrderState GetState()
    {
        if (state == OrderState.Failed || state == OrderState.Complete)
            return state;

        UpdateState();
        return state;
    }

    private void UpdateState()
    {
        var currentStepState = GetCurrentStepState();
        if (currentStepState == OrderStepState.FinishedFailed)
            state = OrderState.Failed;
        else if (currentStep == steps[0] && currentStepState == OrderStepState.None)
            state = OrderState.WaitingToStart;
        else if (currentStepState == OrderStepState.InProgress)
            state = OrderState.InProgress;
        else if (currentStep == steps[^1] && currentStepState == OrderStepState.Finished)
            state = OrderState.Complete;
        else if (currentStepState == OrderStepState.Finished)
            state = OrderState.WaitingToProgress;
    }

    public OrderStepState GetCurrentStepState()
    {
        switch (currentStep)
        {
            case { didStepFail: true }:
                return OrderStepState.FinishedFailed;
            case { isStepFinished: true }:
                return OrderStepState.Finished;
            case { isStepInProgress: true }:
                return OrderStepState.InProgress;
            default:
                return OrderStepState.None;
        }
    }
}
