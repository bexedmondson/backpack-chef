using System;
using Godot;

[GlobalClass, Icon("res://assets/editor/icons/microwave.svg")]
public abstract partial class Equipment : AbstractLoadableDataResource
{
    [Export]
    public string name { get; private set; }

    [Export]
    public Texture2D icon;
    
    [Export]
    public Texture2D smallIcon;

    [Export]
    public PackedScene scene;

    [Export]
    protected double defaultProgress;

    public Action OnChange;
    protected EquipmentManager equipmentManager;

    private void OnCurrentOrderStepUpdated()
    {
        Log.Print(name, true);
        OnChange?.Invoke();
    }

    public virtual double GetProgressPercentDelta(double input = 1)
    {
        return defaultProgress * input;
    }

    public virtual void ProgressCurrentOrder(double percentIncrease)
    {
        equipmentManager ??= Injection.Get<EquipmentManager>();
        var hasOrder = equipmentManager.TryGetOrder(this, out var order);

        if (!hasOrder)
            return;
        
        order.currentStep.MakeProgress(percentIncrease);
    }

    public virtual bool IsOverheating()
    {
        return false;
    }

    public abstract bool HasOrderStepFailed(OrderStep step, double progress);
}
