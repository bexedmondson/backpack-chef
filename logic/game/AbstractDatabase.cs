using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public abstract class AbstractDatabase
{
    public abstract void DoLoad();
}

public abstract class AbstractDatabase<T> : AbstractDatabase where T : AbstractLoadableDataResource
{
    protected abstract string resourceDirectoryPath { get; }

    private List<T> dataItems = new();
    
    public async override void DoLoad()
    {
        List<string> filenames = new List<string>();

        using var dir = DirAccess.Open(resourceDirectoryPath);
        if (dir != null)
        {
            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (fileName != "")
            {
                Log.Print("Found file: " + fileName);
                if (fileName.GetExtension() != ".tres")
                {
                    Log.Print("Skipping file with extension " + fileName.GetExtension());
                    fileName = dir.GetNext();
                    continue;
                }
                filenames.Add(fileName);
                fileName = dir.GetNext();
            }
        }

        foreach (string filename in filenames)
        {
            string filePath = $"{resourceDirectoryPath}/{filename}";
            ResourceLoader.LoadThreadedRequest(filePath, useSubThreads:true, cacheMode: ResourceLoader.CacheMode.Ignore);

            await LoadTask.WaitUntil(() => {
                Log.Print($"Awaiting resource load at path {filePath}");
                return ResourceLoader.LoadThreadedGetStatus(filePath) != ResourceLoader.ThreadLoadStatus.InProgress;
            });

            if (ResourceLoader.LoadThreadedGetStatus(filePath) == ResourceLoader.ThreadLoadStatus.Failed)
            {
                Log.Error($"Resource load failed, path {filePath}");
                continue;
            }

            Log.Print($"Resource load finished, getting file of type {typeof(T).Name} at path {filePath}");
            var loadedResource = (T)ResourceLoader.LoadThreadedGet(filePath);

            loadedResource.PostLoadSetup();
            dataItems.Add(loadedResource);
        }
    }

    public T[] GetItems()
    {
        return dataItems.ToArray();
    }
}
