using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public abstract class AbstractDatabase
{
    public abstract Task DoLoad();
}

public abstract class AbstractDatabase<T> : AbstractDatabase, IInjectable where T : AbstractLoadableDataResource
{
    protected abstract string resourceDirectoryPath { get; }

    private List<T> dataItems = new();

    protected AbstractDatabase()
    {
        RegisterInjection();
    }

    protected abstract void RegisterInjection();
    
    public override async Task DoLoad()
    {
        List<string> filenames = new List<string>();

        using var dir = DirAccess.Open(resourceDirectoryPath);
        if (dir != null)
        {
            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (fileName != "")
            {
                Log.PrintVerbose("Found file: " + fileName);
                if (fileName.GetExtension() != "tres")
                {
                    Log.Print("Skipping file with extension " + fileName.GetExtension());
                    fileName = dir.GetNext();
                    continue;
                }
                filenames.Add(fileName);
                fileName = dir.GetNext();
            }
        }

        var loadTasks = new List<Task>();
        foreach (string filename in filenames)
        {
            string filePath = $"{resourceDirectoryPath}/{filename}";
            loadTasks.Add(DoSingleFileLoad(filePath));
        }

        await Task.WhenAll(loadTasks);
    }

    private async Task<ResourceLoader.ThreadLoadStatus> DoSingleFileLoad(string filePath)
    {
        ResourceLoader.LoadThreadedRequest(filePath, useSubThreads:true);

        await LoadTask.WaitUntil(() => {
            Log.Print($"Awaiting resource load at path {filePath}");
            return ResourceLoader.LoadThreadedGetStatus(filePath) != ResourceLoader.ThreadLoadStatus.InProgress;
        });

        if (ResourceLoader.LoadThreadedGetStatus(filePath) == ResourceLoader.ThreadLoadStatus.Failed)
        {
            Log.Error($"Resource load failed, path {filePath}");
            return ResourceLoader.ThreadLoadStatus.Failed;
        }

        var loadedResource = ResourceLoader.LoadThreadedGet(filePath);
        if (!(loadedResource is T loadedResourceT))
        {
            Log.Error($"Resource load finished, but file at path {filePath} is not type {typeof(T).Name}!");
            return ResourceLoader.ThreadLoadStatus.Failed;
        }
            
        Log.Print($"Resource load finished, getting file of type {typeof(T).Name} at path {filePath}");

        loadedResourceT.PostLoadSetup();
        dataItems.Add(loadedResourceT);
        
        return ResourceLoader.ThreadLoadStatus.Loaded;
    }

    public T[] GetItems()
    {
        return dataItems.ToArray();
    }
}
