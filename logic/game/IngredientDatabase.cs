public class IngredientDatabase : AbstractDatabase<Ingredient>
{
    protected override string resourceDirectoryPath => "res://logic/data/ingredients";
    protected override void RegisterInjection()
    {
        Injection.Register(this);
    }
}