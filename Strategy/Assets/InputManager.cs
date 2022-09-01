using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;

    private void FixedUpdate()
    {
        TogglePauseMenu();
    }

    private void TogglePauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
        }
    }
}
