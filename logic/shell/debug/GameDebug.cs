using System;

public static class GameDebug
{
    private static bool m_on = false;
    public static bool On {
        get => m_on;
        set {
            if (m_on != value)
            {
                m_on = value;
                OnGameDebugToggled?.Invoke();
                Log.Warn("debug: " + value);
            }
        }
    }
    
    public static event Action OnGameDebugToggled;
}