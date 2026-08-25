using System;
using Godot;

public class OrderStep(RecipeStep step)
{
    private RecipeStep recipeStep = step;
    
    public Equipment equipment => recipeStep.equipment;
    public PackedScene visualOverlayStepStart => recipeStep.sceneStepStart;
    public PackedScene visualOverlayStepEnd => recipeStep.sceneStepEnd;
    
    public bool isStepInProgress { get; private set; }
    public bool isStepFinished { get; private set; }
    public bool didStepFail { get; private set; }

    public float progressPercent { get; private set; } = 0;

    public Action OnStepCompleted;

    public void StartStep()
    {
        isStepInProgress = true;
    }

    public void MakeProgress(float percentIncrease)
    {
        progressPercent += percentIncrease;
        if (progressPercent >= 100 && !isStepFinished)
        {
            CompleteStep();
        }
    }

    public PackedScene GetVisualOverlayScene()
    {
        return this.isStepFinished ? visualOverlayStepEnd : visualOverlayStepStart;
    }

    public void CompleteStep()
    {
        isStepInProgress = false;
        isStepFinished = true;
        OnStepCompleted?.Invoke();
    }

    public void FailStep()
    {
        didStepFail = true;
    }
}
