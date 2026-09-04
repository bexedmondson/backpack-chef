using System;
using Godot;

public partial class OrderDisplay : Control
{
    [Export]
    private Label recipeNameLabel;
    
    [Export]
    private TextureRect recipeIcon;

    [Export]
    private InstancePlaceholder stepDisplayPlaceholder;

    [Export]
    private ProgressBar timeRemainingBar;

    [Export]
    private ScrollContainer scrollContainer;

    [Export]
    private VBoxContainer scrollChild;

    [Export]
    private AnimationPlayer animationPlayer;

    [Export]
    private Godot.Collections.Dictionary<OrderState, string> orderEndAnimationNameMap = new();

    private Order order;

    public void Setup(Order newOrder)
    {
        this.order = newOrder;
        recipeNameLabel.Text = GameDebug.On ? order.recipe.name : string.Empty;
        
        recipeIcon.Texture = order.recipe.icon;

        for (int i = 0; i < order.steps.Length; i++)
        {
            var step = order.steps[i];
            var newStepDisplay = stepDisplayPlaceholder.CreateInstance() as OrderStepDisplay;
            newStepDisplay.SetStep(i, step);
        }

        timeRemainingBar.MaxValue = Mathf.RoundToInt(timeRemainingBar.Size.X);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (order == null || order.GetState().IsEnded())
            return;
        
        timeRemainingBar.Value = order.GetTimeRemainingProportion() * timeRemainingBar.MaxValue;
    }

    public async void DoOrderRemovalAnimation(Action AnimationFinishedAction)
    {
        var orderState = order.GetState();

        var animationStateToPlay = orderEndAnimationNameMap[orderState];
        
        animationPlayer.Play(animationStateToPlay);
        await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
        
        AnimationFinishedAction?.Invoke();
    }

    /*public override Variant _GetDragData(Vector2 atPosition)
    {
        SetDragPreview(GetDragPreview());
        return this;
    }

    private Control GetDragPreview()
    {
        var dupe = this.Duplicate() as Control;

        dupe.Size = this.Size;
        
        this.Modulate = Colors.Transparent;
        return dupe;
    }*/

    public void AnimateBackgroundHeight()
    {
        scrollContainer.Size = scrollChild.Size;
        scrollContainer.CustomMinimumSize = scrollChild.Size;
        
        var tween = CreateTween();
        tween.SetEase(Tween.EaseType.InOut);
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetParallel(true);
        tween.TweenProperty(scrollContainer, "size", new Vector2(scrollContainer.Size.X, 0), 0.6);
        tween.TweenProperty(scrollContainer, "custom_minimum_size", new Vector2(scrollContainer.CustomMinimumSize.X, 0), 0.6);
        tween.Play();
    }
    
    public override void _Notification(int what)
    {
        if (what == NotificationDragEnd)
        {
            this.Modulate = Colors.White;
        }
    }
}
