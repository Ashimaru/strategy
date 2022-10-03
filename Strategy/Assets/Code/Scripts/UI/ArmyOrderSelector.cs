using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct Operation
{
    public Operation(string name, Action action)
    {
        Name = name;
        Action = action;
    }

    public string Name { get; set; }
    public Action Action { get; set; }
}

public class ArmyOrderSelector : MonoBehaviour
{
    [SerializeField]
    private Transform _orderButtonParent;
    [SerializeField]
    private GameObject _orderPrefab;

    [SerializeField]
    List<GameObject> _createdOptions = new();


    void Start()
    {
        Hide();
        //Utils.CreateRepeatingTimer(gameObject, 1f, () =>
        //{
        Vector3 position = UnityEngine.Random.insideUnitCircle;
        position *= 3f;
        position.z = transform.position.z;
        ShowOptions(new List<Operation>()
        {
            new Operation(){Name = "Move", Action = () => Debug.Log("Moving!")},
            new Operation(){Name = "Other", Action = () => Debug.Log("Othering!") }
        }, position);
        //},
        //"Test");
    }

    public void ShowOptions(List<Operation> operations, Vector3 position)
    {
        ClearOptions();
        position.z = transform.position.z;
        transform.position = position;

        foreach (var operation in operations)
        {
            var go = Instantiate(_orderPrefab, _orderButtonParent);
            go.name = operation.Name;
            var button = go.GetComponent<Button>();
            button.onClick.AddListener(() => operation.Action.Invoke());

            var text = go.GetComponentInChildren<TextMeshProUGUI>();
            text.text = operation.Name;
            _createdOptions.Add(go);
        }
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ClearOptions()
    {
        foreach (var option in _createdOptions)
        {
            Destroy(option);
        }
        _createdOptions.Clear();
    }
}
