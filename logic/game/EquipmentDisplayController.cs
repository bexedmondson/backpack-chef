using Godot;

public partial class EquipmentDisplayController : Node
{
    [Export]
    private Godot.Collections.Array<EquipmentDisplay> equipmentDisplays = new();

    public override void _Ready()
    {
        base._Ready();
        var allEquipment = Injection.Get<EquipmentDatabase>().GetItems();

        for (int i = 0; i < equipmentDisplays.Count; i++)
        {
            if (i > allEquipment.Length - 1)
                break;

            equipmentDisplays[i].SetEquipment(allEquipment[i]);
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        
        foreach (var equipmentDisplay in equipmentDisplays)
        {
            equipmentDisplay.QueueFree();
        }
        equipmentDisplays.Clear();
    }
}
