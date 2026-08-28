using System.Collections.Generic;
using Godot;

public partial class EquipmentDisplayController : Node, IInjectable
{
    [Export]
    private Godot.Collections.Array<Control> equipmentDisplayParents = new();
    
    private Dictionary<Equipment, EquipmentDisplay> equipmentDisplays = new();

    public override void _EnterTree()
    {
        base._EnterTree();
        Injection.Register(this);
    }

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
            
            equipmentDisplays[allEquipment[i]] = equipmentInstance;
            
            equipmentDisplayParents[i].AddChild(equipmentInstance);
        }
    }

    public void RefreshEquipmentDisplay(Equipment equipment)
    {
        equipmentDisplays[equipment].RefreshCurrentOrderDisplay();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        
        foreach (var kvp in equipmentDisplays)
        {
            kvp.Value.QueueFree();
        }
        equipmentDisplays.Clear();
        
        Injection.Deregister(this);
    }
}
