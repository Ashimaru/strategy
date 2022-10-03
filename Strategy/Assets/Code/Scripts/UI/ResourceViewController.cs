using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;





public class ResourceViewController : MonoBehaviour
{
    private class ResourceView
    {
        Label _label;
        Resource _resource;

        public ResourceView(Label label, Resource resource)
        {
            _label = label;
            _resource = resource;
            _resource.OnAmountChanged += UpdateValue;
            UpdateValue(0, _resource.Amount);
        }

        ~ResourceView()
        {
            _resource.OnAmountChanged -= UpdateValue;
        }

        void UpdateValue(int oldValue, int newValue)
        {
            Debug.Log("Update with value " + newValue);
            _label.text = newValue.ToString();
        }
    }

    private VisualElement root;
    private PlayerResources _playerResources;
    private List<ResourceView> _resourcesView = new List<ResourceView>();

    private void Awake()
    {
        Debug.Log("ResourceViewController Awake");

        _playerResources = FindObjectOfType<PlayerResources>();
    }

    private void OnEnable()
    {
        Debug.Log("ResourceViewController OnEnable");

        root = GetComponent<UIDocument>().rootVisualElement;

        _resourcesView.Add(new ResourceView(root.Q<Label>("bodies-value"), _playerResources.Resources.Bodies));
        _resourcesView.Add(new ResourceView(root.Q<Label>("bones-value"), _playerResources.Resources.Bones));
        _resourcesView.Add(new ResourceView(root.Q<Label>("gold-value"), _playerResources.Resources.Gold));
        _resourcesView.Add(new ResourceView(root.Q<Label>("mana-value"), _playerResources.Resources.Mana));
    }
}
