using System.Threading.Tasks;
using Godot;

public class GameSceneManager : IInjectable
{
    private Node loadedSceneNode = null;

    public GameSceneManager()
    {
        Injection.Register(this);
    }

    public void InstantiateScene(PackedScene loadedScene)
    {
        if (loadedSceneNode != null)
        {
            Log.Error("Game scene manager doesn't handle multiple game scenes yet!!");
        }

        var gameSceneInstance = loadedScene.Instantiate();
        loadedSceneNode = gameSceneInstance;

        loadedSceneNode.SetProcess(false);
    }

    public void AddSceneNodeToTree()
    {   
        var sceneTreeAccessor = Injection.Get<SceneTreeAccessor>();
        sceneTreeAccessor.activeSceneNode = loadedSceneNode;
        sceneTreeAccessor.currentSceneTree.Root.AddChild(loadedSceneNode);

        loadedSceneNode.SetProcess(true);
    }

    public async Task RemoveActiveSceneNode()
    {
        if (loadedSceneNode != null)
        {
            loadedSceneNode.QueueFree();
            loadedSceneNode = null;
        }

        var sceneTreeAccessor = Injection.Get<SceneTreeAccessor>();

        await sceneTreeAccessor.currentSceneTree.ToSignal(sceneTreeAccessor.currentSceneTree, SceneTree.SignalName.NodeRemoved);
        sceneTreeAccessor.activeSceneNode = null;
    }

    public bool HasActiveGameSceneNodeInTree => loadedSceneNode != null && loadedSceneNode.IsInsideTree() && loadedSceneNode is GameController;
    public bool HasActiveSaveSelectSceneNodeInTree => loadedSceneNode != null && loadedSceneNode.IsInsideTree() && loadedSceneNode is SaveSelectScreen;
}