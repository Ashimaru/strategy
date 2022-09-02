using SaveSystem;
using UnityEngine;

public class CameraMovementController : MonoBehaviour, SaveSystem.ISaveable
{
    private Vector3 origin;
    private Vector3 difference;

    private readonly Rect screenSpaceBounds = new Rect(Screen.width * 0.01f, Screen.height * 0.01f, Screen.width * 0.98f, Screen.height * 0.98f);

    [SerializeField]
    private float scrollSpeed = 15;

    void LateUpdate()
    {
        if (DragCamera())
        {
            return;
        }
#if !UNITY_EDITOR
        ScrollCameraIfCoursorNearTheEdge();
#endif
    }

    bool DragCamera()
    {
        if (Input.GetMouseButtonDown(2))
        {
            origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return true;
        }

        if (Input.GetMouseButton(2))
        {
            difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            Camera.main.transform.position = origin - difference;
            return true;
        }

        return false;
    }

    void ScrollCameraIfCoursorNearTheEdge()
    {
        var pos = Input.mousePosition;
        var screenSpacePosition = new Vector2(pos.x, pos.y);
        if (!screenSpaceBounds.Contains(screenSpacePosition))
        {
            var direction = new Vector3(screenSpacePosition.x, screenSpacePosition.y) - new Vector3(screenSpaceBounds.center.x, screenSpaceBounds.center.y);
            direction.Normalize();
            Camera.main.transform.position += direction * Time.deltaTime * scrollSpeed;
        }
    }

    [System.Serializable]
    public class SaveData
    {
        public Vector3 position;

        public SaveData()
        {
            position = new Vector3(0,0,-10);
        }
    }

    public object SaveState()
    {
        return new SaveData()
        {
            position = transform.position
        };
    }

    public void LoadState(object savedState)
    {
        var saveData = (SaveData)savedState;
        transform.position = saveData.position;
    }
}
