using System;
using System.IO;
using Godot;

public static class Log
{
    public static bool showDetailedLogs { get; private set; } = false;

    public static void PrintVerbose(string log,
        bool includeMethod = false,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", 
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
    {
        log = PrependCallSourceInfo(log, includeMethod, memberName, sourceFilePath);
        if (showDetailedLogs)
            log = PrependCallSourceInfo(log, includeMethod, memberName, sourceFilePath);
    }
    
    public static void PrintVerbose(string log, Color color,
        bool includeMethod = false,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", 
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
    {
        if (showDetailedLogs)
            Print(log, color, includeMethod, memberName, sourceFilePath);
    }
    
    public static void Print(string log,
        bool includeMethod = false,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", 
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
    {
        log = PrependCallSourceInfo(log, includeMethod, memberName, sourceFilePath);
        GD.Print($"{DateTime.Now} {log}");
    }
    
    public static void Print(string log, Color color,
        bool includeMethod = false,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", 
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
    {
        log = PrependCallSourceInfo(log, includeMethod, memberName, sourceFilePath);
        GD.PrintRich($"{DateTime.Now} [color={color.ToHtml().ToLower()}]{log}[/color]");
    }

    public static void Warn(string log,
        bool includeMethod = false,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", 
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
    {
        log = PrependCallSourceInfo(log, includeMethod, memberName, sourceFilePath);
        GD.PrintRich($"[color=yellow]{DateTime.Now} {log}[/color]");
        GD.PushWarning(log);
    }
    
    public static void Error(string log,
        bool includeMethod = true,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", 
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
    {
        log = PrependCallSourceInfo(log, includeMethod, memberName, sourceFilePath);
        GD.PrintErr($"{DateTime.Now.ToLongTimeString()}\t {log}");
        GD.PushError(log);
    }

    private static string PrependCallSourceInfo(string log, bool includeMethod, string memberName, string sourceFilePath)
    {
        var fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
        
        if (includeMethod)
            return $"[{fileName}] {memberName}: {log}";
        
        return $"[{fileName}] {log}";
    }
}
