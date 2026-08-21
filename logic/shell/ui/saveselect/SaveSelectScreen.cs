using Godot;

public partial class SaveSelectScreen : Control
{
    [Export]
    private Container saveButtonContainer;

    [Export]
    private PackedScene saveButtonScene;

    [Export]
    private SaveDeleteConfirmationPopup deleteConfirmPopup;

    public override void _EnterTree()
    {
        base._EnterTree();

        SetupSaveButtons();
    }

    private void SetupSaveButtons()
    {
        deleteConfirmPopup.Visible = false;
        
        var saves = Injection.Get<SaveManager>().GetAvailableSaveDirectories();
        if (saves != null)
        {
            foreach (string save in saves)
            {
                var saveButton = saveButtonScene.Instantiate<SelectSaveButton>();
                saveButton.SetSavePath(save);
                saveButton.AddDeleteCallback(OnDeleteButton);
                saveButtonContainer.AddChild(saveButton);
            }
        }

        if (saves == null || saves.Length < 8)
        {
            var newGameButton = saveButtonScene.Instantiate<SelectSaveButton>();
            newGameButton.SetSavePath(string.Empty);
            saveButtonContainer.AddChild(newGameButton);
        }
    }

    private void OnDeleteButton(string savePathToDelete)
    {
        deleteConfirmPopup.Setup(savePathToDelete, OnSaveDeleted);
        deleteConfirmPopup.Visible = true;
    }

    private void OnSaveDeleted()
    {
        for (int i = saveButtonContainer.GetChildCount() - 1; i >= 0; i--)
        {
            saveButtonContainer.GetChild(i).QueueFree();
        }
        
        SetupSaveButtons();
    }
}
