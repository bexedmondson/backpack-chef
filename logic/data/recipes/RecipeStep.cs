using Godot;
using Godot.Collections;

[GlobalClass]
public partial class RecipeStep : Resource
{
    [Export]
    public Equipment equipment { get; private set; }
    
    [Export]
    public Array<Ingredient> ingredientsRequired { get; private set; }
}
