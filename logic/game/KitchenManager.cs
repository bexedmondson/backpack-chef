public class KitchenManager: AbstractManager
{
    public KitchenManager()
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

    public override void UpdateSaveFile() { }

    public override void LoadFromSaveFile() { }
}