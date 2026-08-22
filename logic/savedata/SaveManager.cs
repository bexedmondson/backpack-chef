using System;
using System.IO;
using System.Threading.Tasks;
using Godot;
using FileAccess = Godot.FileAccess;

public class SaveManager : IInjectable
{
    private static readonly string s_defaultSaveContainerDirectory = "res://logic/savedata";
    private static readonly string s_saveDataContainerFileName = "default_save.tres";
    private static readonly string s_saveDirectoryPath = "user://saves";
    private string nameOfSelectedSaveDirectory = string.Empty;

    private SaveData saveDataContainer;
    
    public SaveManager()
    {
        Injection.Register(this);
    }
    
    public string[] GetAvailableSaveDirectories()
    {
        string[] directories = null;
        if (DirAccess.DirExistsAbsolute(s_saveDirectoryPath))
        {
            directories = DirAccess.GetDirectoriesAt(s_saveDirectoryPath);
            foreach (var directory in directories)
                Log.Print($"Found save at {directory}");
        }
        else
        {
            DirAccess.MakeDirAbsolute(s_saveDirectoryPath);
        }

        return directories;
    }

    public void SetSelectedSave(string savePath)
    {
        nameOfSelectedSaveDirectory = savePath;
        Injection.Get<EventDispatcher>().Dispatch(new SaveSelectedEvent());
    }

    public async Task LoadSelectedSave()
    {
        string directoryToLoadFrom;
        if (string.IsNullOrEmpty(nameOfSelectedSaveDirectory))
        {
            directoryToLoadFrom = s_defaultSaveContainerDirectory;
        }
        else if (!DirAccess.DirExistsAbsolute(Path.Combine(s_saveDirectoryPath, nameOfSelectedSaveDirectory)))
        {
            Log.Error($"Trying to load save from {Path.Combine(s_saveDirectoryPath, nameOfSelectedSaveDirectory)} but directory doesn't exist!");
            directoryToLoadFrom = s_defaultSaveContainerDirectory;
        }
        else
        {
            directoryToLoadFrom = Path.Combine(s_saveDirectoryPath, nameOfSelectedSaveDirectory);
        }

        var saveContainerFileToLoad = Path.Combine(directoryToLoadFrom, s_saveDataContainerFileName);
        if (!ResourceLoader.Exists(saveContainerFileToLoad))
        {
            Log.Error($"No save container file found at {saveContainerFileToLoad}!");
            return;
        }
        
        ResourceLoader.LoadThreadedRequest(saveContainerFileToLoad, cacheMode: ResourceLoader.CacheMode.Ignore);

        await LoadTask.WaitUntil(() => {
            Log.Print($"Awaiting resource load at path {saveContainerFileToLoad}");
            return ResourceLoader.LoadThreadedGetStatus(saveContainerFileToLoad) != ResourceLoader.ThreadLoadStatus.InProgress;
        });

        if (ResourceLoader.LoadThreadedGetStatus(saveContainerFileToLoad) == ResourceLoader.ThreadLoadStatus.Failed)
        {
            Log.Error($"Resource load failed, path {saveContainerFileToLoad}");
            return;
        }

        Log.Print($"Resource load finished, getting file at path {saveContainerFileToLoad}");
        saveDataContainer = (SaveData)ResourceLoader.LoadThreadedGet(saveContainerFileToLoad);

        if (saveContainerFileToLoad.Contains(s_defaultSaveContainerDirectory))
        {
            var newSaveDirectory = Path.Combine(s_saveDirectoryPath, DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss"));
            DirAccess.MakeDirAbsolute(newSaveDirectory);
            
            foreach (var saveData in saveDataContainer.saveDatas)
            {
                var newFilePath = Path.Combine(newSaveDirectory, saveData.fileName + ".tres");
                ResourceSaver.Save(saveData, newFilePath);
                saveData.TakeOverPath(newFilePath);
                Log.Print($"Saved copy of default save file at {newFilePath}");
            }
            
            var newSaveContainerPath = Path.Combine(newSaveDirectory, s_saveDataContainerFileName);
            ResourceSaver.Save(saveDataContainer, newSaveContainerPath);
            //saveDataContainer.TakeOverPath(newSaveContainerPath);
            Log.Print($"Saved copy of default save file container in new directory at {newSaveContainerPath}");
        }
    }

    public T GetLoadedSaveData<T>() where T : AbstractSaveData
    {
        foreach (var saveData in saveDataContainer.saveDatas)
        {
            if (saveData is T saveDataT)
                return saveDataT;
        }
        
        Log.Error($"Save data of type {typeof(T).Name} not found! Using default save data");

        var defaultSaveContainerFilePath = Path.Combine(s_defaultSaveContainerDirectory, s_saveDataContainerFileName);
        var defaultSaveContainer = ResourceLoader.Load<SaveData>(defaultSaveContainerFilePath);
        foreach (var saveData in defaultSaveContainer.saveDatas)
        {
            if (saveData is not T saveDataT)
                continue;
            
            var newFilePath = Path.Combine(s_saveDirectoryPath, nameOfSelectedSaveDirectory, saveDataT.fileName + ".tres");
            ResourceSaver.Save(saveDataT, newFilePath);
            saveDataT.TakeOverPath(newFilePath);
            Log.Print($"Saved copy of default save file at {newFilePath}");

            saveDataContainer.saveDatas.Add(saveDataT);
            ResourceSaver.Save(saveDataContainer);
            Log.Print($"Added reference to copy of default save file in save container at {saveDataContainer.ResourcePath}");

            return saveDataT;
        }
        
        Log.Error($"Default save data of type {typeof(T).Name} not found! Returning null!");
        return null;
    }

    public void SavePartial<T>(T saveData) where T : AbstractSaveData
    {
        if (!saveDataContainer.saveDatas.Contains(saveData))
        {
            var savePath = Path.Combine(s_saveDirectoryPath, nameOfSelectedSaveDirectory, saveData.fileName + ".tres");
            Log.Print($"Saving file of type {saveData.GetType().Name} {saveData} to {savePath}");
            ResourceSaver.Save(saveData, savePath);
            saveDataContainer.saveDatas.Add(saveData);
            ResourceSaver.Save(saveDataContainer);
        }
        else
        {
            ResourceSaver.Save(saveData);
            Log.Print($"Saving file of type {saveData.GetType().Name} {saveData} to {saveData.ResourcePath}");
        }
    }

    public void SaveAll()
    {
        RNG.Save();
        foreach (var saveData in saveDataContainer.saveDatas)
        {
            ResourceSaver.Save(saveData);
        }
    }

    public void SaveChanged()
    {
        RNG.Save();
        foreach (var saveData in saveDataContainer.saveDatas)
        {
            if (saveData.changed)
                ResourceSaver.Save(saveData);
        }
    }

    public void DeleteSave(string nameOfSaveToDelete)
    {
        var deletePath = Path.Combine(s_saveDirectoryPath, nameOfSaveToDelete);
        if (DirAccess.DirExistsAbsolute(deletePath))
        {
            Log.Print($"Deleting save at {deletePath}", Colors.Red);
            OS.MoveToTrash(ProjectSettings.GlobalizePath(deletePath));
        }
    }
}
