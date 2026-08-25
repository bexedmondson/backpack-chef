using System.Collections.Generic;

public class Order
{
    public Recipe recipe { get; private set;  }

    private List<OrderStep> orderSteps = new();
    public OrderStep[] steps => orderSteps.ToArray();
    
    public OrderStep currentStep { get; private set; }
    
    private float timeRemaining;

    public OrderState state = OrderState.Waiting;

    public void SetRecipe(Recipe recipe)
    {
        this.recipe = recipe;

        foreach (var recipeStep in recipe.steps)
        {
            var orderStep = new OrderStep(recipeStep);
            orderSteps.Add(orderStep);
        }
        
        currentStep = orderSteps[0];
    }

    public OrderStep GetNextStep()
    {
        if (state == OrderState.Waiting)
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
}
