using System.Threading.Tasks;
using Godot;

public abstract class AbstractStateLoadScene : AbstractState
{
    protected abstract string GetScenePath();
    
    protected override async Task<bool> DoStateTasksAsync()
    {
        var initialGameScenePath = GetScenePath();

        var eventDispatcher = Injection.Get<EventDispatcher>();
        
        ResourceLoader.LoadThreadedRequest(initialGameScenePath, cacheMode: ResourceLoader.CacheMode.Ignore);

        Godot.Collections.Array progress = new Godot.Collections.Array();
        while (ResourceLoader.LoadThreadedGetStatus(initialGameScenePath, progress) == ResourceLoader.ThreadLoadStatus.InProgress)
        {
            Log.PrintVerbose($"{Name}: awaiting scene load, {progress[0]}");
            eventDispatcher.Dispatch(new SceneLoadProgressUpdateEvent((float)progress[0]));
            //have to do this because ResourceLoader doesn't have anything awaitable
            await Task.Delay(25);
        }

        var scene = (PackedScene)ResourceLoader.LoadThreadedGet(initialGameScenePath);
        
        GameSceneManager gameSceneManager = Injection.Get<GameSceneManager>();
        gameSceneManager.InstantiateScene(scene);
        gameSceneManager.AddSceneNodeToTree();

        return true;
    }
}