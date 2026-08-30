using System.Collections.Generic;

public class EquipmentManager : AbstractManager
{
    private EquipmentDatabase database;
    private OrderManager orderManager;
    private EquipmentDisplayController equipmentDisplayController;
    
    private Dictionary<Equipment, Order> equipmentCurrentOrderMap = new();

    protected override void RegisterInjection()
    {
        Injection.Register(this);
    }

    public override void Setup()
    {
        database = Injection.Get<EquipmentDatabase>();
        orderManager = Injection.Get<OrderManager>();

        var items = database.GetItems();
        foreach (var equipment in items)
        {
            equipmentCurrentOrderMap[equipment] = null;
        }
    }
    
    public bool CanOrderMoveToEquipment(Order order, Equipment equipment)
    {
        if (equipmentCurrentOrderMap[equipment] != null)
            return false;

        var orderState = order.GetState();
        if (orderState is OrderState.InProgress or OrderState.Failed or OrderState.FailedBinned)
            return false;
        
        var nextOrderStep = order.GetNextStep();
        return nextOrderStep.equipment == equipment;
    }

    public void MoveOrderToEquipment(Order order, Equipment target)
    {
        if (equipmentCurrentOrderMap[target] != null)
        {
            Log.Error($"Trying to move order {order.recipe.name} to equipment {target.name} but already has order {equipmentCurrentOrderMap[target].recipe.name}!");
            return;
        }
        
        var hasCurrentEquipment = equipmentCurrentOrderMap.ContainsValue(order);
        Equipment currentEquipment = null;
        if (hasCurrentEquipment)
        {
            foreach (var kvp in equipmentCurrentOrderMap)
            {
                if (kvp.Value != order)
                    continue;
                
                //TODO notify equipment to update itself
                currentEquipment = kvp.Key;
                break;
            }
            equipmentCurrentOrderMap[currentEquipment] = null;
        }

        equipmentCurrentOrderMap[target] = order;
        orderManager.OnOrderMovedToEquipment(target, order);

        equipmentDisplayController ??= Injection.Get<EquipmentDisplayController>();
        
        if (hasCurrentEquipment)
            equipmentDisplayController.RefreshEquipmentDisplay(currentEquipment);
        equipmentDisplayController.RefreshEquipmentDisplay(target);
    }

    public void RemoveOrderFromAllEquipment(Order order)
    {
        if (!TryGetEquipmentWithOrder(order, out var equipment))
            return;

        equipmentCurrentOrderMap[equipment] = null;
        
        equipmentDisplayController ??= Injection.Get<EquipmentDisplayController>();
        equipmentDisplayController.RefreshEquipmentDisplay(equipment);
    }

    public bool TryGetEquipmentWithOrder(Order order, out Equipment equipment)
    {
        equipment = null;
        foreach (var kvp in equipmentCurrentOrderMap)
        {
            if (kvp.Value != order)
                continue;
            
            equipment = kvp.Key;
            break;
        }

        return equipment != null;
    }

    public bool HasOrder(Equipment equipment)
    {
        return equipmentCurrentOrderMap[equipment] != null;
    }
    
    public bool TryGetOrder(Equipment equipment, out Order order)
    {
        order = equipmentCurrentOrderMap[equipment];
        return order != null;
    }

    public override void Cleanup()
    {
        Injection.Deregister(this);
        equipmentCurrentOrderMap.Clear();
        database = null;
        orderManager = null;
    }
}
