public class OrderStep(RecipeStep step)
{
    private RecipeStep recipeStep = step;
    
    public Equipment equipment => recipeStep.equipment;
    
    public bool isStepInProgress { get; private set; }
    public bool isStepFailed => false;

    public void StartStep()
    {
        isStepInProgress = true;
    }

    public void CompleteStep()
    {
        isStepInProgress = false;
    }
}
