using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;
    [SerializeField]
    GameObject loadGameScreen;
    [SerializeField]
    GameObject saveGameScreen;

    private void Update()
    {
        TogglePauseMenu();
    }

    private void TogglePauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            if(loadGameScreen.activeSelf)
            {
                loadGameScreen.SetActive(false);
            }
            if(saveGameScreen.activeSelf)
            {
                saveGameScreen.SetActive(false);
            }
        }
    }
}
