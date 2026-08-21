using System;
using Godot;

public partial class SaveDeleteConfirmationPopup : Control
{
    [Export]
    private Label deleteConfirmPathLabel;

    private string savePathToDelete;
    private Action OnSaveDeletedCallback;
    
    public void Setup(string savePathToDelete, Action onSaveDeleted)
    {
        this.savePathToDelete = savePathToDelete;
        deleteConfirmPathLabel.Text = savePathToDelete;
        OnSaveDeletedCallback = onSaveDeleted;
    }

    public void OnDeleteConfirmed()
    {
        Injection.Get<SaveManager>().DeleteSave(savePathToDelete);
        OnSaveDeletedCallback?.Invoke();

        OnSaveDeletedCallback = null;
        deleteConfirmPathLabel.Text = string.Empty;
        savePathToDelete = null;
    }

    public void OnDeleteCancelled()
    {
        OnSaveDeletedCallback = null;
        deleteConfirmPathLabel.Text = string.Empty;
        savePathToDelete = null;
        this.Visible = false;
    }
}
