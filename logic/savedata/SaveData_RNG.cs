using Godot;

[GlobalClass]
public partial class SaveData_RNG : AbstractSaveData
{
    public override string fileName => "rng";
    
    [Export]
    public ulong seed;

    [Export]
    public ulong state;
}