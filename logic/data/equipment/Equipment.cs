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
    public float defaultProgress { get; private set; }

    public Action OnChange;

    public Order currentOrder { get; private set; }

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        Injection.Get<EventDispatcher>().Add<OrderStateChangedEvent>(OnOrderStateChanged);
    }

    public void SetCurrentOrder(Order order)
    {
        currentOrder = order;
        order.currentStep.OnStepCompleted += OnCurrentOrderStepUpdated;
        
        Log.Print($"{name} new order set {order.recipe.name}", true);
        OnChange?.Invoke();
    }

    private void OnOrderStateChanged(OrderStateChangedEvent orderStateChangedEvent)
    {
        //if this is to do with another order than the one that's at this equipment, i don't care
        if (orderStateChangedEvent.order != currentOrder)
            return;
        
        Log.Print($"{name} {currentOrder.recipe.name}", true);
        
        //if the order that's here is currently actually at other equipment, remove it from here!
        if (currentOrder.currentStep.equipment != this)
        {
            Log.Print($"{name} removing current order ({currentOrder.recipe.name})");
            currentOrder = null;
            OnChange?.Invoke();
        }
    }

    private void OnCurrentOrderStepUpdated()
    {
        Log.Print(name, true);
        OnChange?.Invoke();
    }

    public virtual float GetProgressPercent(float input = 0)
    {
        return defaultProgress * input;
    }
}
