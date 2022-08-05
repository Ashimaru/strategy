using UnityEngine;

public enum UnitType
{
    Melee,
    Ranged
}

[CreateAssetMenu(fileName = "UnitData", menuName ="Game/UnitType")]
public class UnitData : ScriptableObject
{
    [SerializeField]
    private string UnitTypeName_;
    [SerializeField]
    private UnitType UnitType_;
    [SerializeField]
    private int MeeleAttack_;
    [SerializeField]
    private int RangedAttack_;
    [SerializeField]
    private int MaxHP_;

    public string UnitTypeName { get => UnitTypeName_;}
    public int MeeleAttack { get => MeeleAttack_;}
    public int RangedAttack { get => RangedAttack_;}
    public UnitType UnitType { get => UnitType_; }
    public int MaxHP { get => MaxHP_; } 
}
