using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName ="Game/UnitType")]
public class UnitData : ScriptableObject
{
    [SerializeField]
    private string UnitTypeName_;
    [SerializeField]
    private int MeeleAttack_;
    [SerializeField]
    private int RangedAttack_;

    public string UnitTypeName { get => UnitTypeName_;}
    public int MeeleAttack { get => MeeleAttack_;}
    public int RangedAttack { get => RangedAttack_;}
}
