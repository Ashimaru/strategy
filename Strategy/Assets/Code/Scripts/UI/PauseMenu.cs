using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private string mainMenuSceneName;

    private void Awake()
    {
        //gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        rootVisualElement.Q<Button>("save-game").RegisterCallback<MouseUpEvent>(evt => SaveGame());
        rootVisualElement.Q<Button>("load-game").RegisterCallback<MouseUpEvent>(evt => LoadGame());
        rootVisualElement.Q<Button>("quit-to-main-menu").RegisterCallback<MouseUpEvent>(evt => QuitToMainMenu());
        rootVisualElement.Q<Button>("quit-to-desktop").RegisterCallback<MouseUpEvent>(evt => QuitToDesktop());
    }

    private void SaveGame()
    {
        Debug.Log("Save Game");
    }

    private void LoadGame()
    {
        Debug.Log("Load Game");
    }

    private void QuitToMainMenu()
    {
        Debug.Log("Quit To Main Menu");
        SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Single);
    }

    private void QuitToDesktop()
    {
        Debug.Log("Quit To Desktop");
    }
}
