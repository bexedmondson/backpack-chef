using System.Collections.Generic;

public class OrderManager : AbstractManager
{
    //placeholder values for testing with :)
    //TODO REPLACE
    private float orderSeparationTime = 10f;
    private int totalLevelOrders = 10;

    private RecipeAvailabilityManager recipeAvailabilityManager;
    private GameTimeMonitor timeMonitor;
    private double timeSinceLastOrderCreated = 0f;

    private List<Order> currentOrders = new();
    
    protected override void RegisterInjection()
    {
        Injection.Register(this);
    }

    public override void Setup()
    {
        eventDispatcher = Injection.Get<EventDispatcher>();
        recipeAvailabilityManager = Injection.Get<RecipeAvailabilityManager>();
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

        if (totalLevelOrders <= currentOrders.Count)
            return;
        
        MakeNewOrder();
    }

    private void MakeNewOrder()
    {
        Log.PrintVerbose($"Orders still to generate: {totalLevelOrders - currentOrders.Count}", true);

        var newOrder = new Order();
        
        var availableRecipes = recipeAvailabilityManager.GetAvailableRecipes();
        //TODO change to use RNG at least, if not overhaul entirely
        var selectedRecipe = availableRecipes[RNG.RandiRange(0, availableRecipes.Count - 1)];
        newOrder.SetRecipe(selectedRecipe);

        currentOrders.Add(newOrder);
        timeSinceLastOrderCreated = 0;
        eventDispatcher.Dispatch(new OrderCreatedEvent(newOrder));
    }

    public override void Cleanup()
    {
        recipeAvailabilityManager = null;
        eventDispatcher = null;
        Injection.Deregister(this);
    }
}
