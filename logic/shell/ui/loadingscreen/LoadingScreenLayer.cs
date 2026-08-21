using Godot;

public partial class LoadingScreenLayer : CanvasLayer
{
    [Export]
    private ProgressBar progressBar;

    public void StartListeningForProgress()
    {
        Injection.Get<EventDispatcher>().Add<SceneLoadProgressUpdateEvent>(UpdateProgressBar);
    }

    private void UpdateProgressBar(SceneLoadProgressUpdateEvent e)
    {
        float progress = Mathf.RoundToInt(e.progress * 100) / 100f;
        if (progress > 0.95f)
            progress = 0.95f;
        progressBar.Value = progress;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        
        Injection.Get<EventDispatcher>().Remove<SceneLoadProgressUpdateEvent>(UpdateProgressBar);
    }
}
