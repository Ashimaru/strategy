using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        rootVisualElement.Q<Button>("new-game").RegisterCallback<MouseUpEvent>(evt => NewGame());
        rootVisualElement.Q<Button>("load-game").RegisterCallback<MouseUpEvent>(evt => LoadGame());
        rootVisualElement.Q<Button>("quit-game").RegisterCallback<MouseUpEvent>(evt => Quit());

    }


    private void NewGame()
    {
        Debug.Log("New Game");
    }

    private void LoadGame()
    {
        Debug.Log("Load Game");
    }

    private void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
