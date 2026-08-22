using Godot;
using Godot.Collections;

[GlobalClass, Icon("res://assets/editor/icons/notebook-tabs.svg")]
public partial class Recipe : AbstractLoadableDataResource
{
    [Export]
    public string name { get; private set; }

    [Export]
    public Array<Ingredient> ingredientsRequired { get; private set; }

    [Export]
    public Array<RecipeStep> steps { get; private set; }
}
