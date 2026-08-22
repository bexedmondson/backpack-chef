public class KitchenManager: AbstractManager
{
    protected override void RegisterInjection()
    {
        Injection.Register(this);
    }

    public override void Setup()
    {
        
    }

    public override void Cleanup()
    {
        Injection.Deregister(this);
    }
}