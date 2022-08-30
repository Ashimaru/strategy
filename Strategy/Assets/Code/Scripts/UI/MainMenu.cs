using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private string newGameSceneName;
    [SerializeField]
    private GameObject loadGameScreen;

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
        SceneManager.LoadScene(newGameSceneName, LoadSceneMode.Single);
    }

    private void LoadGame()
    {
        Debug.Log("Load Game");
        loadGameScreen.SetActive(true);
    }

    private void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
