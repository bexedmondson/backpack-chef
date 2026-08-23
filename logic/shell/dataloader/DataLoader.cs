using Godot;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class DataLoader : IInjectable
{
    private bool m_isInitialised = false;
    
    private List<AbstractDatabase> databases = new();

    public DataLoader()
    {
        Injection.Register(this);
        databases.Add(new RecipeDatabase());
        databases.Add(new IngredientDatabase());
        databases.Add(new EquipmentDatabase());
    }

    public async Task LoadAllResources()
    {
        var loadTasks = new List<Task>();
        foreach (var database in databases)
        {
            loadTasks.Add(database.DoLoad());
        }

        await Task.WhenAll(loadTasks);
    }
}