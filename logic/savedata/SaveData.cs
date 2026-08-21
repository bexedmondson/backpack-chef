using Godot;

[GlobalClass]
public partial class SaveData : Resource
{
    [Export]
    public Godot.Collections.Array<AbstractSaveData> saveDatas;
}
