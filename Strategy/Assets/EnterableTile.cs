using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnterableTile
{
    public abstract void OnArmyEnter(ArmyController army);
}
