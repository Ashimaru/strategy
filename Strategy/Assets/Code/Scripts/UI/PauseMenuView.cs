using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseMenuView : MonoBehaviour
{
    [SerializeField]
    private string mainMenuSceneName;
    IPauseUIView pauseUIView;

    private void Awake()
    {
        pauseUIView = GetComponentInParent<IPauseUIView>();
        if(pauseUIView == null)
        {
            Debug.LogError("Parent is missing IPauseUIView component");
        }
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
        pauseUIView.TransitionToSaveGameView();
    }

    private void LoadGame()
    {
        Debug.Log("Load Game");
        pauseUIView.TransitionToLoadGameView();
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
