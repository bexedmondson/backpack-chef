public abstract class AbstractManager : IInjectable, ISaverLoader
{
    protected EventDispatcher eventDispatcher;
    protected SaveManager saveManager;
    
    public abstract void Setup();
    public abstract void Cleanup();
    public abstract void UpdateSaveFile();
    public abstract void LoadFromSaveFile();
}
