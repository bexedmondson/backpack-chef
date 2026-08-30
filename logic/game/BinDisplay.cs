using Godot;

public partial class BinDisplay : TextureRect
{
    [Export]
    private Texture2D defaultTexture;
    
    [Export]
    private Texture2D hoverTexture;
    
    private OrderManager orderManager;
    private OrderDisplayController orderDisplayController;

    public override void _EnterTree()
    {
        base._EnterTree();
        orderManager = Injection.Get<OrderManager>();
        orderDisplayController = Injection.Get<OrderDisplayController>();

        this.MouseEntered += OnMouseEntered;
        this.MouseExited += OnMouseExited;
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        orderManager = null;
        orderDisplayController = null;
    }
    
    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        if (data.As<GodotObject>() is not OrderDisplay)
            return false;
        return true; //TODO add some kind of timer so player has to hold it over the bin for a second before successfully dropping it 
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        base._DropData(atPosition, data);
        if (data.As<GodotObject>() is not OrderDisplay orderDisplay)
            return;

        orderDisplayController ??= Injection.Get<OrderDisplayController>();
        
        orderManager.BinOrder(orderDisplayController.GetOrderForDisplay(orderDisplay));
    }

    private void OnMouseEntered()
    {
        this.Texture = hoverTexture;
    }

    private void OnMouseExited()
    {
        this.Texture = defaultTexture;
    }
}
