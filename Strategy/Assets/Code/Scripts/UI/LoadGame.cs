using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

struct LoadGameData
{
    public string name;
    public string date;
    public LoadGameData(string name, string date)
    {
        this.name = name;
        this.date = date;
    }
}

public class LoadGame : MonoBehaviour
{
    private VisualElement root;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("exit-load-game-screen").RegisterCallback<MouseUpEvent>(evt => ExitLoadGameScreen());
        FillLoadGameList();
    }

    private void ExitLoadGameScreen()
    {
        Debug.Log("Exit Load Game Screen");
        gameObject.SetActive(false);
    }

    private void FillLoadGameList()
    {

        var loadGameList = root.Q<ListView>("load-game-list");
        //loadGameList.hierarchy.Add(new Label("This is hardcoded example"));

        // Create a list of data. In this case, numbers from 1 to 1000.
        const int itemCount = 5;
        var items = new List<LoadGameData>(itemCount);
        for (int i = 1; i <= itemCount; i++)
            items.Add(new LoadGameData("This is hardcoded example (" + i.ToString() + ") ", "28/08/2022"));

        loadGameList.makeItem = () => new Label();
        loadGameList.bindItem = (e, i) => {
            var label = (Label)e;
            label.text = items[i].name + "\n" + items[i].date;
            label.style.fontSize = 20;
            label.style.color = Color.white;
        };
        //// The "makeItem" function is called when the
        //// ListView needs more items to render.
        //Func<VisualElement> makeItem = () => new Label();

        //// As the user scrolls through the list, the ListView object
        //// recycles elements created by the "makeItem" function,
        //// and invoke the "bindItem" callback to associate
        //// the element with the matching data item (specified as an index in the list).
        //Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = items[i];

        //// Provide the list view with an explict height for every row
        //// so it can calculate how many items to actually display
        //const int itemHeight = 16;

        //var listView = new ListView(items, itemHeight, makeItem, bindItem);

        //listView.selectionType = SelectionType.Multiple;

        loadGameList.onItemsChosen += objects => Debug.Log(objects);
        loadGameList.onSelectionChange += objects => Debug.Log(objects);

        loadGameList.itemsSource = items;

        //listView.style.flexGrow = 1.0f;

        //root.Add(listView);
    }
}
