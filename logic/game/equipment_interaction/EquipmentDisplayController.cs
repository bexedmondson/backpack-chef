using System.Collections.Generic;
using Godot;

public partial class EquipmentDisplayController : Node
{

    [Export]
    private Godot.Collections.Array<Control> equipmentDisplayParents = new();
    
    private List<EquipmentDisplay> equipmentDisplays = new();

    public override void _Ready()
    {
        base._Ready();
        var allEquipment = Injection.Get<EquipmentDatabase>().GetItems();

        for (int i = 0; i < equipmentDisplayParents.Count; i++)
        {
            if (i > allEquipment.Length - 1)
                break;

            var equipmentInstance = allEquipment[i].scene.Instantiate() as EquipmentDisplay;
            equipmentInstance.SetEquipment(allEquipment[i]);
            equipmentDisplays.Add(equipmentInstance);
            equipmentDisplayParents[i].AddChild(equipmentInstance);
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
