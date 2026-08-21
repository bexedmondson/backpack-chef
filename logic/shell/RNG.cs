using Godot;

public static class RNG
{
    private static RandomNumberGenerator rng = new();

    public static float Randf()
    {
        return rng.Randf();
    }
    
    public static float RandfRange(float inclusiveStart, float inclusiveEnd)
    {
        return rng.RandfRange(inclusiveStart, inclusiveEnd);
    }
    
    public static int RandiRange(int inclusiveStart, int inclusiveEnd)
    {
        return rng.RandiRange(inclusiveStart, inclusiveEnd);
    }

    public static void Save()
    {
        var saveManager = Injection.Get<SaveManager>();
        SaveData_RNG saveData = saveManager.GetLoadedSaveData<SaveData_RNG>();
        saveData.seed = rng.Seed;
        saveData.state = rng.State;
    }

    public static void Load()
    {
        var saveManager = Injection.Get<SaveManager>();
        SaveData_RNG saveData = saveManager.GetLoadedSaveData<SaveData_RNG>();
        rng = new RandomNumberGenerator();

        if (saveData.seed != 0)
        {
            rng.Seed = saveData.seed;
            rng.State = saveData.state;
        }
        else
        {
            rng.Randomize();
        }
    }
}
