public class Order
{
    public Recipe recipe { get; private set;  }
    public RecipeStep currentStep { get; private set;  }
    private float timeRemaining;

    public OrderState state = OrderState.Waiting;

    public void SetRecipe(Recipe recipe)
    {
        this.recipe = recipe;
        currentStep = recipe.steps[0];
    }

    public RecipeStep GetNextStep()
    {
        if (state == OrderState.Waiting)
            return currentStep;
        
        int currentStepIndex = recipe.steps.IndexOf(currentStep);
        int nextStepIndex = currentStepIndex + 1;
        if (nextStepIndex > (recipe.steps.Count - 1))
        {
            Log.Error($"Next step index is {nextStepIndex}, recipe step count is {recipe.steps.Count}! Returning null.", true);
            return null;
        }
        
        return recipe.steps[nextStepIndex];
    }
}
