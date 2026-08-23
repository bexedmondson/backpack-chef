public class Order
{
    public Recipe recipe { get; private set;  }
    private RecipeStep currentStep;
    private float timeRemaining;

    public OrderState state = OrderState.Waiting;

    public void SetRecipe(Recipe recipe)
    {
        this.recipe = recipe;
        currentStep = recipe.steps[0];
    }
}
