using Godot;

public abstract partial class EquipmentDisplay : Control
{
    [Export]
    private Control orderDisplayContainer;

    [Export]
    private Node currentOrderStepVisualOverlayParent;

    [Export]
    private Control readyIndicator;

    [Export]
    private Control smokeParticleParent;

    [Export]
    private Godot.Collections.Array<AnimatedSprite2D> smokeParticles = new();

    protected EquipmentManager equipmentManager;
    protected OrderDisplayController orderDisplayController;
    
    protected Equipment equipment;
    protected OrderDisplay currentOrderDisplay;
    protected Control currentOrderStepVisualOverlay;
    
    public override void _EnterTree()
    {
        base._EnterTree();
        equipmentManager = Injection.Get<EquipmentManager>();
        orderDisplayController = Injection.Get<OrderDisplayController>();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        equipmentManager = null;
        orderDisplayController = null;
    }

    public virtual void SetEquipment(Equipment e)
    {
        equipment = e;
        equipment.OnChange += RefreshCurrentOrderDisplay;
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        if (data.As<GodotObject>() is not OrderDisplay orderDisplay)
            return false;
        
        orderDisplayController ??= Injection.Get<OrderDisplayController>();
        
        return equipmentManager.CanOrderMoveToEquipment(orderDisplayController.GetOrderForDisplay(orderDisplay), equipment);
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        base._DropData(atPosition, data);
        if (data.As<GodotObject>() is not OrderDisplay orderDisplay)
            return;
        
        currentOrderDisplay = orderDisplay;
        orderDisplay.Reparent(orderDisplayContainer);
        
        orderDisplayController ??= Injection.Get<OrderDisplayController>();
        
        equipmentManager.MoveOrderToEquipment(orderDisplayController.GetOrderForDisplay(orderDisplay), equipment);
    }

    public virtual void RefreshCurrentOrderDisplay()
    {
        var hasOrder = equipmentManager.TryGetOrder(this.equipment, out var currentOrderAtEquipment);
        if (!hasOrder)
        {
            currentOrderDisplay = null; //other things should be handling cleanup (e.g. OrderDisplayController)
            
            if (currentOrderStepVisualOverlay != null)
            {
                currentOrderStepVisualOverlay.QueueFree();
                currentOrderStepVisualOverlay = null;
            }
            
            readyIndicator.Visible = false;
            return;
        }

        var currentStep = currentOrderAtEquipment.currentStep;

        if (currentOrderStepVisualOverlay == null)
        {
            currentOrderStepVisualOverlay = currentStep.GetVisualOverlayScene().Instantiate<Control>();
            currentOrderStepVisualOverlayParent.AddChild(currentOrderStepVisualOverlay);
            readyIndicator.Visible = false;
            return;
        }

        if (currentStep.isStepFinished)
        {
            if (currentStep.didStepFail)
            {
                currentOrderStepVisualOverlay.Modulate = Colors.DimGray;
                readyIndicator.Visible = false;
            }
            else
            {
                //TODO not the greatest way to swap overlays but not bad for now
                currentOrderStepVisualOverlay.QueueFree();
                currentOrderStepVisualOverlay = currentStep.GetVisualOverlayScene().Instantiate<Control>();
                currentOrderStepVisualOverlayParent.AddChild(currentOrderStepVisualOverlay);

                readyIndicator.Visible = true;
            }
        }
    }

    public void TryMakeProgress()
    {
        if (CanTryMakingProgress())
            MakeProgress();
    }

    protected virtual bool CanTryMakingProgress()
    {
        return equipmentManager.HasOrder(equipment);
    }

    protected abstract void MakeProgress();


    protected virtual void StartSmokeAnimation()
    {
        smokeParticleParent.Visible = true;
        foreach (var smokeParticle in smokeParticles)
        {
            smokeParticle.Visible = false;
        }
        
        var tween = CreateTween();
        for (int i = 0; i < smokeParticles.Count; i++)
        {
            var smokeParticle = smokeParticles[i];
            tween.TweenCallback(Callable.From(() => StartSmokeParticle(smokeParticle)));
            tween.TweenInterval(0.3 * (i + 1));
        }
    }

    private void StartSmokeParticle(AnimatedSprite2D smokeParticle)
    {
        smokeParticle.Visible = true;
    }
}
