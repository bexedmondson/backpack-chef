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
        if (order.currentStep.equipment != equipment)
            order.MoveToNextStep();
        
        if (order.GetCurrentStepState() == OrderStepState.None)
            order.currentStep.StartStep();
        
        eventDispatcher.Dispatch(new OrderStateChangedEvent(order));
    }

    public bool CanOrderMoveToEquipment(Order order, Equipment equipment)
    {
        if (order.currentStep.isStepInProgress || order.currentStep.didStepFail) //TODO figure out action to remove from equipment after failing
            return false;
        
        var nextOrderStep = order.GetNextStep();
        return nextOrderStep.equipment == equipment;
    }

    public override void Cleanup()
    {
        GameDebug.OnGameDebugToggled -= OnDebugToggled;
        recipeAvailabilityManager = null;
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
