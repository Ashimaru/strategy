
public class TransferableGroupView : ArmyGroupView
{
    public ArmyCreatorViewController armyCreator;
    private SoldierGroup soldierGroup;

    private void Start()
    {
        armyCreator = GetComponentInParent<ArmyCreatorViewController>();
    }
    public override void LoadSoldierGroup(SoldierGroup soldierGroup)
    {
        base.LoadSoldierGroup(soldierGroup);
        this.soldierGroup = soldierGroup;
    }

    public void TransferToOtherGroup()
    {
        armyCreator.MoveGroupToOtherArmy(soldierGroup);
    }
}
