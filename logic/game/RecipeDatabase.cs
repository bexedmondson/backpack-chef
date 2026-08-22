public class RecipeDatabase : AbstractDatabase<Recipe>
{
    protected override string resourceDirectoryPath => "res://logic/data/recipes";
    protected override void RegisterInjection()
    {
        Injection.Register(this);
    }
}