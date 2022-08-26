using UnityEngine;
using UnityEngine.Tilemaps;

interface IArmyFactory
{
    GameObject CreateArmy(Army army, Vector3Int position);
    GameObject CreateAIArmy(Army army, Vector3Int position);
}

class ArmyFactory : MonoBehaviour, IArmyFactory
{
    [SerializeField]
    private GameObject armyPrefab;
    [SerializeField]
    private ArmyViewController armyViewController;
    [SerializeField]
    private Tilemap unitsTilemap;
    [SerializeField]
    private TileBase armyTile;

    private void Awake()
    {
        Systems.RegisterSystem<IArmyFactory>(this);
    }

    public GameObject CreateArmy(Army army, Vector3Int position)
    {
        var armyInstance = Instantiate(armyPrefab);
        armyInstance.name = army.ArmyName;
        var armyController = armyInstance.GetComponent<ArmyController>();
        armyController.army = army;
        armyController.armyViewController = armyViewController;
        armyController.unitTilemap = unitsTilemap;
        armyController.armyTile = armyTile;
        armyController.CurrentPosition = position;

        return armyInstance;
    }

    public GameObject CreateAIArmy(Army army, Vector3Int position)
    {
        var armyGo = CreateArmy(army, position);
        armyGo.AddComponent<AIArmyController>();
        return armyGo;
    }
}

