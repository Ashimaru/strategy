using SaveSystem;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ZoomController : MonoBehaviour, ISaveable
{
    [SerializeField]
    private float minZoom, maxZoom, zoomSpeed = 1F;

    private Camera _camera;

    void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        _camera.orthographicSize -= Input.mouseScrollDelta.y * zoomSpeed;
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, minZoom, maxZoom);
    }

    [System.Serializable]
    public class SaveData
    {
        public float orthographicSize;

        public SaveData()
        {
            orthographicSize = 8;
        }
    }

    public object SaveState()
    {
        return new SaveData()
        {
            orthographicSize = _camera.orthographicSize
        };
    }

    public void LoadState(object savedState)
    {
        var saveData = (SaveData)savedState;
        _camera.orthographicSize = saveData.orthographicSize;
    }
}
