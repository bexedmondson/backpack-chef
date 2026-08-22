using Godot;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class DataLoader : IInjectable
{
    private bool m_isInitialised = false;

    private JsonSerializerOptions m_serializerOptions = new JsonSerializerOptions{
        Converters = {
            new JsonStringEnumConverter()
        }
    };
    private List<AbstractDatabase> databases = new();

    public DataLoader()
    {
        Injection.Register(this);
        databases.Add(new RecipeDatabase());
        databases.Add(new IngredientDatabase());
        databases.Add(new EquipmentDatabase());
    }

    public Task LoadAllResources()
    {
        foreach (var database in databases)
        {
            database.DoLoad();
        }
        
        return Task.CompletedTask;
    }
}