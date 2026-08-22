public abstract class AbstractManager : IInjectable, ISaverLoader
{
    protected EventDispatcher eventDispatcher;
    protected SaveManager saveManager;

    protected AbstractManager()
    {
        RegisterInjection();
    }

    protected abstract void RegisterInjection();
    
    public abstract void Setup();
    public abstract void Cleanup();
    public virtual void UpdateSaveFile() { }
    public virtual void LoadFromSaveFile() { }
}
