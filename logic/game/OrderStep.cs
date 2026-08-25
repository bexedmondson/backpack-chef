using System;
using Godot;

public class OrderStep(RecipeStep step)
{
    private RecipeStep recipeStep = step;
    
    public Equipment equipment => recipeStep.equipment;
    public PackedScene equipmentVisualOverlay => recipeStep.sceneStepStart;
    
    public bool isStepInProgress { get; private set; }
    public bool isStepFinished { get; private set; }
    public bool didStepFail { get; private set; }

    public Action OnStepCompleted;

    public void StartStep()
    {
        isStepInProgress = true;
    }

    public void CompleteStep()
    {
        isStepInProgress = false;
        isStepFinished = true;
        OnStepCompleted?.Invoke();
    }
}
