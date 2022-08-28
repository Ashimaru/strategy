using UnityEngine;

public enum UnitType
{
    Melee,
    Ranged
}

[CreateAssetMenu(fileName = "UnitData", menuName = "Game/UnitType")]
public class UnitData : ScriptableObject
{
    [SerializeField]
    private string UnitTypeName_;
    [SerializeField]
    private UnitType UnitType_;
    [SerializeField]
    private int Attack_;
    [SerializeField]
    private int MaxHP_;

    [SerializeField]
    private JobData _creationJob;

    public string UnitTypeName { get => UnitTypeName_; }
    public int Attack { get => Attack_; }
    public UnitType UnitType { get => UnitType_; }
    public int MaxHP { get => MaxHP_; }
    public JobData CreationJob { get => _creationJob; }
}
