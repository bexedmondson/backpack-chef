using System.Collections.Generic;
using System.Text;
using Godot;

public class OrderManager : AbstractManager
{
    //placeholder values for testing with :)
    //TODO REPLACE
    private float orderSeparationTime = 5f;
    private int totalLevelOrders = 10;

    private RecipeAvailabilityManager recipeAvailabilityManager;
    private GameTimeMonitor timeMonitor;
    private EquipmentManager equipmentManager;
    private OrderDisplayController orderDisplayController;
    private double timeSinceLastOrderCreated = 0f;

    private List<Order> currentLevelOrders = new();
    
    protected override void RegisterInjection()
    {
        Injection.Register(this);
    }

    public override void Setup()
    {
        eventDispatcher = Injection.Get<EventDispatcher>();
        recipeAvailabilityManager = Injection.Get<RecipeAvailabilityManager>();
        equipmentManager = Injection.Get<EquipmentManager>();

        GameDebug.OnGameDebugToggled += OnDebugToggled;
    }

    public void OnGameStart(GameTimeMonitor gameTimeMonitor)
    {
        timeSinceLastOrderCreated = orderSeparationTime;
        this.timeMonitor = gameTimeMonitor;
        timeMonitor.OnProcess += OnGameTicked;
    }

    private void OnGameTicked(double delta)
    {
        timeSinceLastOrderCreated += delta;
        if (timeSinceLastOrderCreated < orderSeparationTime)
            return;

        if (totalLevelOrders <= currentLevelOrders.Count)
            return;
        
        MakeNewOrder();
    }

    private void MakeNewOrder()
    {
        Log.PrintVerbose($"Orders still to generate: {totalLevelOrders - currentLevelOrders.Count}", true);

        var newOrder = new Order();
        
        var availableRecipes = recipeAvailabilityManager.GetAvailableRecipes();
        //TODO change to use RNG at least, if not overhaul entirely
        var selectedRecipe = availableRecipes[RNG.RandiRange(0, availableRecipes.Count - 1)];
        newOrder.SetRecipe(selectedRecipe);

        currentLevelOrders.Add(newOrder);
        timeSinceLastOrderCreated = 0;
        eventDispatcher.Dispatch(new OrderCreatedEvent(newOrder));
    }

    public void OnOrderMovedToEquipment(Equipment equipment, Order order)
    {
        order.MoveToNextStep();
        
        if (order.GetCurrentStepState() == OrderStepState.None)
            order.currentStep.StartStep();
        
        eventDispatcher.Dispatch(new OrderStateChangedEvent(order));
    }

    public void ServeCompleteOrder(Order order)
    {
        
        //TODO give out reward here :)
        
        var isAtEquipment = Injection.Get<EquipmentManager>().TryGetEquipmentWithOrder(order, out var equipment);
        
        equipmentManager.RemoveOrderFromAllEquipment(order);
        
        if (isAtEquipment)
            Injection.Get<EquipmentDisplayController>().RefreshEquipmentDisplay(equipment);
        
        orderDisplayController ??= Injection.Get<OrderDisplayController>();
        orderDisplayController.OnOrderCompleted(order);
    }

    public void BinOrder(Order order)
    {
        var isAtEquipment = Injection.Get<EquipmentManager>().TryGetEquipmentWithOrder(order, out var equipment);
        
        equipmentManager.RemoveOrderFromAllEquipment(order);
        
        if (isAtEquipment)
            Injection.Get<EquipmentDisplayController>().RefreshEquipmentDisplay(equipment);

        orderDisplayController ??= Injection.Get<OrderDisplayController>();
        orderDisplayController.OnOrderBinned(order);
        
        order.Bin();
    }

    public override void Cleanup()
    {
        GameDebug.OnGameDebugToggled -= OnDebugToggled;
        recipeAvailabilityManager = null;
        equipmentManager = null;
        eventDispatcher = null;
        Injection.Deregister(this);
    }

    private void OnDebugToggled()
    {
        if (!GameDebug.On)
            return;

        StringBuilder sb = new();
        sb.AppendLine($"Total order count for level: {totalLevelOrders}");
        sb.AppendLine($"Current order count for level: {currentLevelOrders.Count}");

        foreach (var order in currentLevelOrders)
        {
            sb.AppendLine($"[b]Order: {order.recipe.name}[/b]");
            foreach (var step in order.steps)
            {
                sb.Append(step.isStepInProgress ? "[color=yellow]" : step.didStepFail ? "[color=red]" : step.isStepFinished ? "[color=grey]" : "");
                
                sb.AppendLine($"\tStep: {step.equipment.name}");
                sb.AppendLine($"\t\tIn progress? {step.isStepInProgress}");
                sb.AppendLine($"\t\tFinished? {step.isStepFinished}");
                sb.AppendLine($"\t\tFailed? {step.didStepFail}");
                
                sb.Append((step.isStepInProgress || step.isStepFinished || step.didStepFail) ? "[/color]" : "");
            }
        }
        
        Log.Print(sb.ToString(), Colors.White);
    }
}
