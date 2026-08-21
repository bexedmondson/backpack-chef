
using Godot;

public class StateMachine
{
    public AbstractState currentState { get; private set; }

    public void Begin()
    {
        GameDebug.OnGameDebugToggled += OnDebugToggled;
        InitialiseStates(out var initialState);

        TransitionToState(initialState);
    }

    private void InitialiseStates(out AbstractState initialState)
    {
        var enableLoadingScreenState = new StateEnableLoadingScreen();
        var initialiseShellSystems = new StateInitialiseShellSystems();
        var loadSaveSelectSceneState = new StateLoadSaveSelectScene();
        var disableLoadingScreenState = new StateDisableLoadingScreen();
        
        var saveSelectRunState = new StateSaveSelectRun();
        var unloadSaveSelectState = new StateUnloadSaveSelectScene();
        
        var loadConfigDataState = new StateLoadConfigData();
        var loadSaveDataState = new StateLoadSaveData();
        var gameSetupState = new StateGameSetup();
        var loadGameSceneState = new StateLoadGameScene();
        
        var gameRunState = new StateGameRun();
        var unloadGameSceneState = new StateUnloadGameScene();
        var gameTeardownState = new StateGameTeardown();
        
        initialState = enableLoadingScreenState;
        
        enableLoadingScreenState.AddDefaultStateTransition(initialiseShellSystems);
        initialiseShellSystems.AddDefaultStateTransition(loadSaveSelectSceneState);
        loadSaveSelectSceneState.AddDefaultStateTransition(disableLoadingScreenState);
        disableLoadingScreenState.AddDefaultStateTransition(saveSelectRunState);
        
        saveSelectRunState.AddDefaultStateTransition(enableLoadingScreenState);
        enableLoadingScreenState.AddAlternativeStateTransition(new StateTransitionShouldBeginLoadingGame(unloadSaveSelectState));
        unloadSaveSelectState.AddDefaultStateTransition(loadConfigDataState);
        loadConfigDataState.AddDefaultStateTransition(loadSaveDataState);
        loadSaveDataState.AddDefaultStateTransition(gameSetupState);
        gameSetupState.AddDefaultStateTransition(loadGameSceneState);
        loadGameSceneState.AddDefaultStateTransition(disableLoadingScreenState);
        disableLoadingScreenState.AddAlternativeStateTransition(new StateTransitionShouldRunGame(gameRunState));
        
        gameRunState.AddDefaultStateTransition(enableLoadingScreenState);
        enableLoadingScreenState.AddAlternativeStateTransition(new StateTransitionShouldUnloadGameScene(unloadGameSceneState));
        unloadGameSceneState.AddDefaultStateTransition(gameTeardownState);
        gameTeardownState.AddDefaultStateTransition(loadSaveSelectSceneState);
    }

    private void TransitionToState(AbstractState newState)
    {
        if (currentState != null)
        {
            currentState.OnFinished -= TransitionToState;
        }

        currentState = newState;
        currentState.OnFinished += TransitionToState;

        currentState.Run();
    }

    private void OnDebugToggled()
    {
        if (!GameDebug.On)
            return;
        Log.Print($"[StateMachine] Current state: {currentState.GetType().Name}");
    }
}