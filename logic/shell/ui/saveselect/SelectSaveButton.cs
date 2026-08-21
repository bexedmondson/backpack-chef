using System;
using Godot;

public partial class SelectSaveButton : Control
{
    [Export]
    private Button selectButton;
    
    [Export]
    private Control deleteButton;
    
    private string savePath = string.Empty;

    private Action<string> deleteCallback;
    
    public void SetSavePath(string path)
    {
        this.savePath = path;
        selectButton.Text = savePath == string.Empty ? "new game" : savePath;

        deleteButton.Visible = savePath != string.Empty;
    }

    public void AddDeleteCallback(Action<string> callback)
    {
        deleteCallback = callback;
    }

    public void OnClick()
    {
        Injection.Get<SaveManager>().SetSelectedSave(savePath);
    }

    public void OnDeleteButton()
    {
        deleteCallback?.Invoke(savePath);
    }
}
