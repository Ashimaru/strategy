using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SaveGameView : MonoBehaviour
{
    private VisualElement root;
    private ListView saveGameList;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("exit-save-game-screen").RegisterCallback<MouseUpEvent>(evt => ExitLoadGameScreen());
        root.Q<Button>("save-game").RegisterCallback<MouseUpEvent>(evt => SaveGame());
        saveGameList = root.Q<ListView>("save-game-list");
        FillLoadGameList();
    }

    private void ExitLoadGameScreen()
    {
        Debug.Log("Exit Load Game Screen");
        gameObject.SetActive(false);
    }

    private void FillLoadGameList()
    {
        var fileList = FileHandler.GetSaveGameFileList();
        fileList.Insert(0, new SaveGameFile { name = "New Save...", date = "" });

        saveGameList.makeItem = () => new Label();
        saveGameList.bindItem = (e, i) => {
            var label = (Label)e;
            label.text = fileList[i].name + "\n" + fileList[i].date;
            label.style.fontSize = 20;
            label.style.color = Color.white;
        };


        saveGameList.onItemsChosen += objects => Debug.Log(objects);
        saveGameList.onSelectionChange += objects => Debug.Log(objects);

        saveGameList.itemsSource = fileList;
    }

    void SaveGame()
    {
        if (saveGameList.selectedIndex == 0)
        {
            // TODO Implement pop up window for changing save name 
            var saveSystem = Systems.Get<SaveSystem.ISaveSystem>();
            saveSystem.SaveGame("save.sav");
            return;
        }

        var selectedItem = saveGameList.selectedItem;
        if (selectedItem == null)
        {
            Debug.Log("No save was picked");
            return;
        }

        // TODO Implement overwrite pop up window
        {
            var saveItem = (SaveGameFile)selectedItem;
            var saveSystem = Systems.Get<SaveSystem.ISaveSystem>();
            saveSystem.SaveGame(saveItem.name);
        }
    }
}
