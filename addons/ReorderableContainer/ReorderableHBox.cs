using Godot;

[Tool]
[Icon("Icons/reorderable_hbox_icon.svg")]
[GlobalClass]
public partial class ReorderableHBox : ReorderableContainer
{
	protected void _Ready()
	{
		CustomMinimumSize = new Vector2(CustomMinimumSize.X, 0);
		base._Ready();
	}


}