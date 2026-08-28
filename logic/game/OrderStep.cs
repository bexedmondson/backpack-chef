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

    public double progressPercent { get; private set; } = 0;

    public Action OnStepFinished;
    public Action OnStepFailed;

    public void StartStep()
    {
        isStepInProgress = true;
    }

    public void MakeProgress(double percentIncrease)
    {
        progressPercent += percentIncrease;
        Log.Print($"making progress on order step, increasing by {percentIncrease}, progress is now {progressPercent}");
        if (progressPercent >= 100 && !isStepFinished)
        {
            CompleteStep();
        }
        if (this.recipeStep.equipment.HasOrderStepFailed(this, progressPercent))
            FailStep();
    }

    public PackedScene GetVisualOverlayScene()
    {
        return this.isStepFinished ? visualOverlayStepEnd : visualOverlayStepStart;
    }

    public void CompleteStep()
    {
        isStepInProgress = false;
        isStepFinished = true;
        OnStepFinished?.Invoke();
    }

    public void FailStep()
    {
        didStepFail = true;
        OnStepFailed?.Invoke();
    }
}
