using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadGameView : MonoBehaviour
{
    private VisualElement root;
    private ListView loadGameList;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("exit-load-game-screen").RegisterCallback<MouseUpEvent>(evt => ExitLoadGameScreen());
        root.Q<Button>("load-game").RegisterCallback<MouseUpEvent>(evt => LoadGame());
        loadGameList = root.Q<ListView>("load-game-list");
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


        loadGameList.makeItem = () => new Label();
        loadGameList.bindItem = (e, i) => {
            var label = (Label)e;
            label.text = fileList[i].name + "\n" + fileList[i].date;
            label.style.fontSize = 20;
            label.style.color = Color.white;
        };


        loadGameList.onItemsChosen += objects => Debug.Log(objects);
        loadGameList.onSelectionChange += objects => Debug.Log(objects);

        loadGameList.itemsSource = fileList;
    }

    private void LoadGame()
    {
        var selectedItem = loadGameList.selectedItem;
        if(selectedItem == null)
        {
            Debug.Log("No save was picked");
            return;
        }
        var loadItem = (SaveGameFile)selectedItem;
        var saveSystem = Systems.Get<SaveSystem.ISaveSystem>();
        saveSystem.LoadGame(loadItem.name);
    }
}
