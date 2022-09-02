using SaveSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SaveGameView : MonoBehaviour
{
    private VisualElement root;
    private ListView saveGameList;
    private TextField textField;
    private IPauseUIView pauseUiView;

    private void Awake()
    {
        pauseUiView = GetComponentInParent<IPauseUIView>();
        if(pauseUiView == null)
        {
            Debug.LogError("Parent is missing IPauseUIView component");
        }
    }

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("exit-save-game-screen").RegisterCallback<MouseUpEvent>(evt => ExitLoadGameScreen());
        root.Q<Button>("save-game").RegisterCallback<MouseUpEvent>(evt => SaveGame());
        saveGameList = root.Q<ListView>("save-game-list");
        textField = root.Q<TextField>("new-save-game");
        FillLoadGameList();
    }

    private void ExitLoadGameScreen()
    {
        Debug.Log("Exit Load Game Screen");
        pauseUiView.TransitionToPauseMenuView();
    }

    private void FillLoadGameList()
    {
        var fileList = FileHandler.GetSaveGameFileList();

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
        var saveSystem = Systems.Get<SaveSystem.ISaveSystem>();

        var selectedItem = saveGameList.selectedItem;
        if (selectedItem == null)
        {
            var value = textField.value;
            if (value == "")
            {
                return;
            }
            pauseUiView.HideUI();
            saveSystem.SaveGame(value);

            return;
        }

        // TODO Implement overwrite pop up window
        var saveItem = (SaveGameMetaData)selectedItem;
        pauseUiView.HideUI();
        saveSystem.SaveGame(saveItem.name);
    }
}
