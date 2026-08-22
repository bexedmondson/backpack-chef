using System.Collections.Generic;

public class GameManagerContainer : IInjectable
{
    private List<AbstractManager> managers = new();
    
    public GameManagerContainer()
    {
        Injection.Register(this);
    }

    public void RegisterManager(AbstractManager manager)
    {
        managers.Add(manager);
    }

    public void DeregisterManager(AbstractManager manager)
    {
        if (managers.Contains(manager))
            managers.Remove(manager);
        else
            Log.Warn($"Trying to deregister manager {manager} but not registered in GameManagerContainer!");
    }

    public AbstractManager[] GetManagers()
    {
        return managers.ToArray();
    }
    
    public List<T> GetManagersWithInterface<T>()
    {
        List<T> managersWithInterface = new();
        foreach (var manager in managers)
        {
            if (manager is T managerT)
                managersWithInterface.Add(managerT);
        }
        return managersWithInterface;
    }
}
