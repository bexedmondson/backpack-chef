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

    private void OnCurrentOrderStepUpdated()
    {
        Log.Print(name, true);
        OnChange?.Invoke();
    }

    public virtual double GetProgressPercent(double input = 0)
    {
        return defaultProgress * input;
    }

    public void ProgressCurrentOrder(double percentIncrease)
    {
        var hasOrder = Injection.Get<EquipmentManager>().TryGetOrder(this, out var order);

        if (!hasOrder)
            return;
        
        order.currentStep.MakeProgress(percentIncrease);
    }
}
