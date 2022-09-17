using SaveSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadGameView : MonoBehaviour
{
    private VisualElement root;
    private VisualElement screenShotField;
    private ListView loadGameList;
    IPauseUIView pauseUIView;

    private void Awake()
    {
        pauseUIView = GetComponentInParent<IPauseUIView>();
    }

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("exit-load-game-screen").RegisterCallback<MouseUpEvent>(evt => ExitLoadGameScreen());
        root.Q<Button>("load-game").RegisterCallback<MouseUpEvent>(evt => LoadGame());
        screenShotField = root.Q<VisualElement>("screen-shot");
        loadGameList = root.Q<ListView>("load-game-list");
        FillLoadGameList();
    }

    private void ExitLoadGameScreen()
    {
        Debug.Log("Exit Load Game Screen");
        if(pauseUIView != null)
            pauseUIView.TransitionToPauseMenuView();
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

        loadGameList.onSelectedIndicesChange += ShowSaveGameMetaData;
    }

    private void ShowSaveGameMetaData(IEnumerable<int> index)
    {
        var item = (SaveGameMetaData)loadGameList.selectedItem;
        var screenShot = new Texture2D(2,2);
        screenShot.LoadImage(item.screenShot);
        screenShotField.style.backgroundImage = new StyleBackground(screenShot);
    }

    private void LoadGame()
    {
        var selectedItem = loadGameList.selectedItem;
        if(selectedItem == null)
        {
            Debug.Log("No save was picked");
            return;
        }
        var loadItem = (SaveGameMetaData)selectedItem;
        if(pauseUIView != null)
            pauseUIView.HideUI();
        if (SaveSystem.SaveManager.instance == null)
        {
            Debug.LogWarning("Loading only available when running game from PersistantScene");
            return;
        }
        SaveSystem.SaveManager.instance.LoadGame(loadItem.name);
    }
}
