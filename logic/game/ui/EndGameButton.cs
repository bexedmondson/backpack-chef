using Godot;

public partial class EndGameButton : Button
{
    public void OnClick()
    {
        Injection.Get<EventDispatcher>().Dispatch(new RequestExitGameEvent());
    }
}
