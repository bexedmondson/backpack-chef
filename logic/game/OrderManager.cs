public class OrderManager : AbstractManager
{
    //placeholder values for testing with :)
    //TODO REPLACE
    private float orderSeparationTime = 10f;
    private int totalLevelOrders = 10;

    private GameTimeMonitor timeMonitor;
    private double timeSinceLastOrderCreated = 0f;
    private int ordersCreated = 0;
    
    protected override void RegisterInjection()
    {
        Injection.Register(this);
    }

    public override void Setup()
    {
        
    }

    public void OnGameStart(GameTimeMonitor gameTimeMonitor)
    {
        this.timeMonitor = gameTimeMonitor;
        timeMonitor.OnProcess += OnGameTicked;
    }

    private void OnGameTicked(double delta)
    {
        timeSinceLastOrderCreated += delta;
        if (timeSinceLastOrderCreated < orderSeparationTime)
            return;

        if (totalLevelOrders <= ordersCreated)
            return;
        
        MakeNewOrder();
    }

    private void MakeNewOrder()
    {
        Log.PrintVerbose($"Orders still to generate: {totalLevelOrders - ordersCreated}", true);

        

        ordersCreated++;
        timeSinceLastOrderCreated = 0;
    }

    public override void Cleanup()
    {
        Injection.Deregister(this);
    }
}
