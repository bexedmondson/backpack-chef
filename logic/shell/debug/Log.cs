using System;
using Godot;

public static class Log
{
    public static bool showDetailedLogs { get; private set; } = false;

    public static void PrintVerbose(string log)
    {
        if (showDetailedLogs)
            Print(log);
    }
    
    public static void PrintVerbose(string log, string colorName)
    {
        if (showDetailedLogs)
            Print(log, colorName);
    }
    
    public static void Print(string log)
    {
        GD.Print($"{DateTime.Now}\t {log}");
    }
    
    public static void Print(string log, string colorName)
    {
        GD.PrintRich($"{DateTime.Now}\t [color={colorName}]{log}[/color]");
    }

    public static void Warn(string log)
    {
        GD.PrintRich($"[color=yellow]{DateTime.Now}\t {log}[/color]");
        GD.PushWarning(log);
    }
    
    public static void Error(string log)
    {
        GD.PrintErr($"{DateTime.Now.ToLongTimeString()}\t {log}");
        GD.PushError(log);
    }
}
