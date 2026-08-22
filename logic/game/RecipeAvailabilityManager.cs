using System.Collections.Generic;

public class RecipeAvailabilityManager : AbstractManager
{
    private RecipeDatabase recipeDatabase;
    private BackpackManager backpackManager;
    
    protected override void RegisterInjection()
    {
        Injection.Register(this);
    }

    public override void Setup()
    {
        recipeDatabase = Injection.Get<RecipeDatabase>();
        backpackManager = Injection.Get<BackpackManager>();
    }

    public List<Recipe> GetAvailableRecipes()
    {
        var availableRecipes = new List<Recipe>();
        
        var allRecipes = recipeDatabase.GetItems();
        foreach (var recipe in allRecipes)
        {
            bool isAvailable = true;
            foreach (var ingredient in recipe.ingredientsRequired)
            {
                if (!backpackManager.HasIngredient(ingredient))
                {
                    isAvailable = false;
                    break;
                }
            }

            if (!isAvailable)
                continue;

            foreach (var recipeStep in recipe.steps)
            {
                if (!backpackManager.HasEquipment(recipeStep.equipment))
                {
                    isAvailable = false;
                    break;
                }
            }

            if (!isAvailable)
                continue;
            
            availableRecipes.Add(recipe);
        }
        
        return availableRecipes;
    }

    public override void Cleanup()
    {
        recipeDatabase = null;
        backpackManager = null;
        Injection.Deregister(this);
    }
}
