public class EquipmentDatabase : AbstractDatabase<Equipment>
{
    protected override string resourceDirectoryPath => "res://logic/data/equipment";
    protected override void RegisterInjection()
    {
        Injection.Register(this);
    }
}