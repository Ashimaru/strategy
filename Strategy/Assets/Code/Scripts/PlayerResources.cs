using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    [SerializeField]
    private NecromancyResources _resourceData;
    private NecromancyResources _resources;
    public NecromancyResources Resources { get => _resources; }

    void Awake()
    {
        _resources = Instantiate(_resourceData);
    }


}
