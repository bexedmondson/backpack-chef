using System.Collections.Generic;

public class BackpackManager : AbstractManager
{
    private List<Equipment> currentEquipment = new();
    private List<Ingredient> currentIngredients = new();
    
    protected override void RegisterInjection()
    {
        Injection.Register(this);
    }

    public override void Setup()
    {
        //TODO this is placeholder! change!
        currentEquipment.AddRange(Injection.Get<EquipmentDatabase>().GetItems());
        currentIngredients.AddRange(Injection.Get<IngredientDatabase>().GetItems());
    }

    public bool HasIngredient(Ingredient ingredient)
    {
        return currentIngredients.Contains(ingredient);
    }

    public bool HasEquipment(Equipment equipment)
    {
        return currentEquipment.Contains(equipment);
    }

    public override void Cleanup()
    {
        currentEquipment.Clear();
        currentIngredients.Clear();
    }
}
